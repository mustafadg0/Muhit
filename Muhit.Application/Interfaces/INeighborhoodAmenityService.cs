using Muhit.Application.Common;

public interface INeighborhoodAmenityService
{
    Task<BaseResponse<NeighborhoodAmenitySummaryResponse>> GetOrCreateSummaryAsync(int neighborhoodId);
}