using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;
using Muhit.Domain.Entities;
using OpenAI.Chat;

namespace Muhit.Infrastructure.Services;

public class AiNeighborhoodAnalysisService : IAiNeighborhoodAnalysisService
{
    private readonly IConfiguration _configuration;
    private readonly MuhitDbContext _context;

    public AiNeighborhoodAnalysisService(
        IConfiguration configuration,
        MuhitDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(
        GenerateNeighborhoodAiAnalysisRequest request)
    {
        try
        {
            var existingAnalysis = await _context.NeighborhoodAiAnalyses
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.NeighborhoodId == request.NeighborhoodId &&
                    x.IsActive &&
                    !x.IsDeleted);

            if (existingAnalysis != null)
            {
                return new BaseResponse<NeighborhoodAiAnalysisResponse>
                {
                    Success = true,
                    Message = "AI mahalle analizi veritabanından getirildi.",
                    Data = MapToResponse(existingAnalysis)
                };
            }

            var apiKey = _configuration["OpenAI:ApiKey"];
            var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return new BaseResponse<NeighborhoodAiAnalysisResponse>
                {
                    Success = false,
                    Message = "OpenAI API key bulunamadı."
                };
            }

            var client = new ChatClient(model, apiKey);
            var prompt = BuildPrompt(request);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("""
                Sen Muhit360 uygulaması için mahalle yaşam analizi üreten bir asistansın.
                Cevaplarını sadece geçerli JSON formatında üret.
                Skorlar 1 ile 10 arasında double tipinde olmalı.
                Abartılı, hakaret içeren veya kesin suç/güvenlik iddiası içeren ifadeler kullanma.
                """),
                new UserChatMessage(prompt)
            };

            var completion = await client.CompleteChatAsync(messages);
            var json = completion.Value.Content[0].Text;

            var aiResponse = JsonSerializer.Deserialize<NeighborhoodAiAnalysisResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (aiResponse == null)
            {
                return new BaseResponse<NeighborhoodAiAnalysisResponse>
                {
                    Success = false,
                    Message = "AI cevabı parse edilemedi."
                };
            }

            var entity = new NeighborhoodAiAnalysis
            {
                NeighborhoodId = request.NeighborhoodId,

                Summary = aiResponse.Summary,

                SafetyScore = aiResponse.Safety.Score,
                SafetyComment = aiResponse.Safety.Comment,

                TransportScore = aiResponse.Transport.Score,
                TransportComment = aiResponse.Transport.Comment,

                QuietnessScore = aiResponse.Quietness.Score,
                QuietnessComment = aiResponse.Quietness.Comment,

                SocialLifeScore = aiResponse.SocialLife.Score,
                SocialLifeComment = aiResponse.SocialLife.Comment,

                CostScore = aiResponse.Cost.Score,
                CostComment = aiResponse.Cost.Comment,

                BestForJson = JsonSerializer.Serialize(aiResponse.BestFor),
                NotIdealForJson = JsonSerializer.Serialize(aiResponse.NotIdealFor),

                LastUpdatedAt = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            await _context.NeighborhoodAiAnalyses.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = true,
                Message = "AI mahalle analizi oluşturuldu ve veritabanına kaydedildi.",
                Data = aiResponse
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    private static NeighborhoodAiAnalysisResponse MapToResponse(
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
                : JsonSerializer.Deserialize<List<string>>(entity.BestForJson) ?? new List<string>(),

            NotIdealFor = string.IsNullOrWhiteSpace(entity.NotIdealForJson)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(entity.NotIdealForJson) ?? new List<string>()
        };
    }

    private static string BuildPrompt(GenerateNeighborhoodAiAnalysisRequest request)
    {
        return $$"""
        Aşağıdaki mahalle için Muhit360 mobil uygulamasında gösterilecek gerçekçi ve kullanıcı dostu bir mahalle analizi üret.

        Şehir: {{request.CityName}}
        İlçe: {{request.DistrictName}}
        Mahalle: {{request.NeighborhoodName}}

        JSON formatı kesinlikle şu yapıda olsun:

        {
          "summary": "2-4 cümlelik mahalle özeti",
          "safety": {
            "score": 1.0,
            "comment": "güvenlik yorumu"
          },
          "transport": {
            "score": 1.0,
            "comment": "ulaşım yorumu"
          },
          "quietness": {
            "score": 1.0,
            "comment": "sessizlik yorumu"
          },
          "socialLife": {
            "score": 1.0,
            "comment": "sosyal hayat yorumu"
          },
          "cost": {
            "score": 1.0,
            "comment": "yaşam maliyeti yorumu"
          },
          "bestFor": [
            "kimler için uygun"
          ],
          "notIdealFor": [
            "kimler için uygun olmayabilir"
          ]
        }

        Kurallar:
        - Skorlar 1-10 arasında integer olsun.
        - Summary doğal Türkçe olsun.
        - Mahalle hakkında kesin bilgin yoksa genelleme yap ama uydurma spesifik mekan adı verme.
        - "çok tehlikeli", "suç yuvası" gibi iddialı ifadeler kullanma.
        - Sadece JSON döndür, açıklama yazma.
        """;
    }
}