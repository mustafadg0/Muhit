using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;

namespace Muhit.Application.Interfaces;

public interface INeighborhoodDetailService
{
    Task<BaseResponse<NeighborhoodDetailResponse>> GetDetailAsync(
        string city,
        string district,
        string neighborhood);
}