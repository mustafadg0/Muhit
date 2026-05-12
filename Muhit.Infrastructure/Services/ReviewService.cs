using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.DTOs.NeighborhoodReview.Request;
using Muhit.Application.Interfaces;
using Muhit.Domain.Entities;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly MuhitDbContext _context;

    public ReviewService(MuhitDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<NeighborhoodReviewItemResponse>> CreateAsync(
        CreateNeighborhoodReviewRequest request)
    {
        var response = new BaseResponse<NeighborhoodReviewItemResponse>();

        var neighborhoodExists = await _context.Neighborhoods
            .AnyAsync(x =>
                x.Id == request.NeighborhoodId &&
                x.IsActive &&
                !x.IsDeleted);

        if (!neighborhoodExists)
        {
            response.Success = false;
            response.Message = "Mahalle bulunamadı.";
            return response;
        }

        var userExists = await _context.AppUsers
            .AnyAsync(x =>
                x.Id == request.AppUserId &&
                x.IsActive &&
                !x.IsDeleted);

        if (!userExists)
        {
            response.Success = false;
            response.Message = "Kullanıcı bulunamadı.";
            return response;
        }

        var review = new NeighborhoodReview
        {
            NeighborhoodId = request.NeighborhoodId,
            AppUserId = request.AppUserId,
            SafetyScore = request.SafetyScore,
            TransportScore = request.TransportScore,
            QuietnessScore = request.QuietnessScore,
            SocialLifeScore = request.SocialLifeScore,
            CostScore = request.CostScore,
            Comment = request.Comment,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        await _context.NeighborhoodReviews.AddAsync(review);
        await _context.SaveChangesAsync();

        var createdReview = await _context.NeighborhoodReviews
            .AsNoTracking()
            .Include(x => x.AppUser)
            .FirstOrDefaultAsync(x =>
                x.Id == review.Id &&
                x.IsActive &&
                !x.IsDeleted);

        if (createdReview == null)
        {
            response.Success = false;
            response.Message = "Yorum oluşturuldu ancak detay bilgisi getirilemedi.";
            return response;
        }

        response.Success = true;
        response.Message = "Yorum başarıyla oluşturuldu.";
        response.Data = new NeighborhoodReviewItemResponse
        {
            Id = createdReview.Id,
            AppUserId = createdReview.AppUserId,
            UserFullName = createdReview.AppUser.FullName,
            Comment = createdReview.Comment,
            SafetyScore = createdReview.SafetyScore,
            TransportScore = createdReview.TransportScore,
            QuietnessScore = createdReview.QuietnessScore,
            SocialLifeScore = createdReview.SocialLifeScore,
            CostScore = createdReview.CostScore,
            CreatedDate = createdReview.CreatedDate
        };

        return response;
    }
}