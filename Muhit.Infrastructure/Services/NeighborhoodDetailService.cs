using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using Muhit.Domain.Entities;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodDetailService : INeighborhoodDetailService
{
    private readonly MuhitDbContext _context;
    private readonly INeighborhoodAmenityService _amenityService;

    public NeighborhoodDetailService(
        MuhitDbContext context,
        INeighborhoodAmenityService amenityService)
    {
        _context = context;
        _amenityService = amenityService;
    }

    public async Task<BaseResponse<NeighborhoodDetailResponse>> GetDetailAsync(
        string city,
        string district,
        string neighborhood)
    {
        try
        {
            var neighborhoodEntity = await _context.Neighborhoods
                .Include(x => x.District)
                    .ThenInclude(x => x.City)
                .Include(x => x.Reviews)
                    .ThenInclude(x => x.AppUser)
                .Include(x => x.AiAnalysis)
                .FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == neighborhood.ToLower() &&
                    x.District.Name.ToLower() == district.ToLower() &&
                    x.District.City.Name.ToLower() == city.ToLower());

            if (neighborhoodEntity == null)
            {
                return new BaseResponse<NeighborhoodDetailResponse>
                {
                    Success = false,
                    Message = "Mahalle bulunamadı."
                };
            }

            var amenitiesResult = await _amenityService
                .GetOrCreateSummaryAsync(neighborhoodEntity.Id);

            if (!amenitiesResult.Success)
            {
                return new BaseResponse<NeighborhoodDetailResponse>
                {
                    Success = false,
                    Message = amenitiesResult.Message
                };
            }

            var reviews = neighborhoodEntity.Reviews
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

            var reviewSummary = BuildReviewSummary(reviews);

            NeighborhoodAiAnalysisResponse? aiAnalysis = null;

            if (neighborhoodEntity.AiAnalysis != null)
            {
                aiAnalysis = MapAiAnalysis(neighborhoodEntity.AiAnalysis);
            }

            var response = new NeighborhoodDetailResponse
            {
                NeighborhoodId = neighborhoodEntity.Id,

                City = neighborhoodEntity.District.City.Name,
                District = neighborhoodEntity.District.Name,
                Neighborhood = neighborhoodEntity.Name,

                Summary = aiAnalysis?.Summary,

                UserReviewSummary = reviewSummary,

                UserReviews = reviews.Select(x => new NeighborhoodReviewResponse
                {
                    Id = x.Id,
                    UserName = x.AppUser != null
                        ? x.AppUser.FullName
                        : null,

                    SafetyScore = x.SafetyScore,
                    TransportScore = x.TransportScore,
                    QuietnessScore = x.QuietnessScore,
                    SocialLifeScore = x.SocialLifeScore,
                    CostScore = x.CostScore,

                    Comment = x.Comment,
                    CreatedDate = x.CreatedDate
                }).ToList(),

                AiAnalysis = aiAnalysis,

                Amenities = amenitiesResult.Data
            };

            return new BaseResponse<NeighborhoodDetailResponse>
            {
                Success = true,
                Message = "Mahalle detayı başarıyla getirildi.",
                Data = response
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<NeighborhoodDetailResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    private static NeighborhoodUserReviewSummaryResponse BuildReviewSummary(
        List<NeighborhoodReview> reviews)
    {
        if (!reviews.Any())
        {
            return new NeighborhoodUserReviewSummaryResponse
            {
                TotalReviewCount = 0,
                SafetyScore = 0,
                TransportScore = 0,
                QuietnessScore = 0,
                SocialLifeScore = 0,
                CostScore = 0,
                OverallScore = 0
            };
        }

        var safety = reviews.Average(x => x.SafetyScore);
        var transport = reviews.Average(x => x.TransportScore);
        var quietness = reviews.Average(x => x.QuietnessScore);
        var socialLife = reviews.Average(x => x.SocialLifeScore);
        var cost = reviews.Average(x => x.CostScore);

        var overall = new[]
        {
            safety,
            transport,
            quietness,
            socialLife,
            cost
        }.Average();

        return new NeighborhoodUserReviewSummaryResponse
        {
            TotalReviewCount = reviews.Count,

            SafetyScore = Math.Round(safety, 1),
            TransportScore = Math.Round(transport, 1),
            QuietnessScore = Math.Round(quietness, 1),
            SocialLifeScore = Math.Round(socialLife, 1),
            CostScore = Math.Round(cost, 1),

            OverallScore = Math.Round(overall, 1)
        };
    }

    private static NeighborhoodAiAnalysisResponse MapAiAnalysis(
        NeighborhoodAiAnalysis entity)
    {
        return new NeighborhoodAiAnalysisResponse
        {
            Summary = entity.Summary,

            Safety = new NeighborhoodAiCategoryScoreResponse
            {
                Score = entity.SafetyScore,
                Comment = entity.SafetyComment
            },

            Transport = new NeighborhoodAiCategoryScoreResponse
            {
                Score = entity.TransportScore,
                Comment = entity.TransportComment
            },

            Quietness = new NeighborhoodAiCategoryScoreResponse
            {
                Score = entity.QuietnessScore,
                Comment = entity.QuietnessComment
            },

            SocialLife = new NeighborhoodAiCategoryScoreResponse
            {
                Score = entity.SocialLifeScore,
                Comment = entity.SocialLifeComment
            },

            Cost = new NeighborhoodAiCategoryScoreResponse
            {
                Score = entity.CostScore,
                Comment = entity.CostComment
            },

            BestFor = string.IsNullOrWhiteSpace(entity.BestForJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(entity.BestForJson)
                    ?? new List<string>(),

            NotIdealFor = string.IsNullOrWhiteSpace(entity.NotIdealForJson)
                ? new List<string>()
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(entity.NotIdealForJson)
                    ?? new List<string>()
        };
    }
}