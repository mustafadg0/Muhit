using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;

namespace Muhit.Application.Interfaces;

public interface IAiNeighborhoodAnalysisService
{
    Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(GenerateNeighborhoodAiAnalysisRequest request);
}