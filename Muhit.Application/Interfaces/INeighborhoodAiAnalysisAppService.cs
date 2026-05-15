using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;

namespace Muhit.Application.Interfaces;

public interface INeighborhoodAiAnalysisAppService
{
    Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(int neighborhoodId);
}