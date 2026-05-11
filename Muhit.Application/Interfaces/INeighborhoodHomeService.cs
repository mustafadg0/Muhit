using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;

namespace Muhit.Application.Interfaces;

public interface INeighborhoodHomeService
{
    Task<BaseResponse<NeighborhoodHomeResponse>> GetDetailAsync(int neighborhoodId);
}