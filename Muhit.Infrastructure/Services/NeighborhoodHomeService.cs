using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodHomeService : INeighborhoodHomeService
{
    private readonly MuhitDbContext _context;
    private readonly IAiNeighborhoodAnalysisService _aiNeighborhoodAnalysisService;

    public NeighborhoodHomeService(
        MuhitDbContext context,
        IAiNeighborhoodAnalysisService aiNeighborhoodAnalysisService)
    {
        _context = context;
        _aiNeighborhoodAnalysisService = aiNeighborhoodAnalysisService;
    }

    public async Task<BaseResponse<NeighborhoodHomeResponse>> GetDetailAsync(int neighborhoodId)
    {
        var response = new BaseResponse<NeighborhoodHomeResponse>();

        var neighborhood = await _context.Neighborhoods
            .AsNoTracking()
            .Include(x => x.District)
            .ThenInclude(x => x.City)
            .FirstOrDefaultAsync(x =>
                x.Id == neighborhoodId &&
                x.IsActive &&
                !x.IsDeleted &&
                x.District.IsActive &&
                !x.District.IsDeleted &&
                x.District.City.IsActive &&
                !x.District.City.IsDeleted);

        if (neighborhood == null)
        {
            response.Success = false;
            response.Message = "Mahalle bulunamadı.";
            return response;
        }

        var reviews = await _context.NeighborhoodReviews
            .AsNoTracking()
            .Include(x => x.AppUser)
            .Where(x =>
                x.NeighborhoodId == neighborhoodId &&
                x.IsActive &&
                !x.IsDeleted &&
                x.AppUser.IsActive &&
                !x.AppUser.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new NeighborhoodReviewItemResponse
            {
                Id = x.Id,
                AppUserId = x.AppUserId,
                UserFullName = x.AppUser.FullName,
                Comment = x.Comment,
                SafetyScore = x.SafetyScore,
                TransportScore = x.TransportScore,
                QuietnessScore = x.QuietnessScore,
                SocialLifeScore = x.SocialLifeScore,
                CostScore = x.CostScore,
                CreatedDate = x.CreatedDate
            })
            .ToListAsync();

        decimal averageScore = 0;

        if (reviews.Any())
        {
            averageScore = Math.Round(
                reviews.Average(x =>
                    (x.SafetyScore +
                     x.TransportScore +
                     x.QuietnessScore +
                     x.SocialLifeScore +
                     x.CostScore) / 5m),
                1);
        }

        var aiRequest = new GenerateNeighborhoodAiAnalysisRequest
        {
            CityId = neighborhood.District.CityId,
            CityName = neighborhood.District.City.Name,
            DistrictId = neighborhood.DistrictId,
            DistrictName = neighborhood.District.Name,
            NeighborhoodId = neighborhood.Id,
            NeighborhoodName = neighborhood.Name
        };

        var aiResult = await _aiNeighborhoodAnalysisService.GenerateAsync(aiRequest);

        response.Success = true;
        response.Message = "Mahalle ana sayfa verisi başarıyla getirildi.";
        response.Data = new NeighborhoodHomeResponse
        {
            NeighborhoodId = neighborhood.Id,
            NeighborhoodName = neighborhood.Name,
            DistrictId = neighborhood.DistrictId,
            DistrictName = neighborhood.District.Name,
            CityId = neighborhood.District.CityId,
            CityName = neighborhood.District.City.Name,
            ReviewCount = reviews.Count,
            AverageScore = averageScore,
            UserReviews = reviews,
            AiAnalysis = aiResult.Success ? aiResult.Data : null
        };

        return response;
    }
}