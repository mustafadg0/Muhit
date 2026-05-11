using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.DTOs.NeighborhoodReview.Request;

namespace Muhit.Application.Interfaces;

public interface IReviewService
{
    Task<BaseResponse<NeighborhoodReviewItemResponse>> CreateAsync(
        CreateNeighborhoodReviewRequest request);
}