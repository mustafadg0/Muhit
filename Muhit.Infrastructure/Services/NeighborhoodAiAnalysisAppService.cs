using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using Muhit.Domain.Entities;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodAiAnalysisAppService : INeighborhoodAiAnalysisAppService
{
    private readonly MuhitDbContext _context;
    private readonly IAiNeighborhoodAnalysisService _aiService;

    public NeighborhoodAiAnalysisAppService(
        MuhitDbContext context,
        IAiNeighborhoodAnalysisService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(int neighborhoodId)
    {
        var neighborhood = await _context.Neighborhoods
            .Include(x => x.District)
                .ThenInclude(x => x.City)
            .Include(x => x.AiAnalysis)
            .FirstOrDefaultAsync(x => x.Id == neighborhoodId);

        if (neighborhood == null)
        {
            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = false,
                Message = "Mahalle bulunamadı."
            };
        }

        if (neighborhood.AiAnalysis != null)
        {
            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = true,
                Message = "AI analizi zaten mevcut.",
                Data = MapAiAnalysis(neighborhood.AiAnalysis)
            };
        }

        var aiResult = await _aiService.GenerateAsync(new GenerateNeighborhoodAiAnalysisRequest
        {
            CityName = neighborhood.District.City.Name,
            DistrictName = neighborhood.District.Name,
            NeighborhoodName = neighborhood.Name
        });

        if (!aiResult.Success || aiResult.Data == null)
        {
            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = false,
                Message = "AI analizi oluşturulamadı."
            };
        }

        var entity = new NeighborhoodAiAnalysis
        {
            NeighborhoodId = neighborhood.Id,

            Summary = aiResult.Data.Summary,

            SafetyScore = aiResult.Data.Safety.Score,
            SafetyComment = aiResult.Data.Safety.Comment,

            TransportScore = aiResult.Data.Transport.Score,
            TransportComment = aiResult.Data.Transport.Comment,

            QuietnessScore = aiResult.Data.Quietness.Score,
            QuietnessComment = aiResult.Data.Quietness.Comment,

            SocialLifeScore = aiResult.Data.SocialLife.Score,
            SocialLifeComment = aiResult.Data.SocialLife.Comment,

            CostScore = aiResult.Data.Cost.Score,
            CostComment = aiResult.Data.Cost.Comment,

            BestForJson = JsonSerializer.Serialize(aiResult.Data.BestFor),
            NotIdealForJson = JsonSerializer.Serialize(aiResult.Data.NotIdealFor),

            LastUpdatedAt = DateTime.UtcNow
        };

        await _context.NeighborhoodAiAnalyses.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new BaseResponse<NeighborhoodAiAnalysisResponse>
        {
            Success = true,
            Message = "AI analizi başarıyla oluşturuldu.",
            Data = MapAiAnalysis(entity)
        };
    }

    public static NeighborhoodAiAnalysisResponse MapAiAnalysis(NeighborhoodAiAnalysis entity)
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
                : JsonSerializer.Deserialize<List<string>>(entity.BestForJson) ?? new List<string>(),

            NotIdealFor = string.IsNullOrWhiteSpace(entity.NotIdealForJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(entity.NotIdealForJson) ?? new List<string>()
        };
    }
}