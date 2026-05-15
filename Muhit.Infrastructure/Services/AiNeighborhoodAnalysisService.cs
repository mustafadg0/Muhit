using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using OpenAI.Chat;

namespace Muhit.Infrastructure.Services;

public class AiNeighborhoodAnalysisService : IAiNeighborhoodAnalysisService
{
    private readonly IConfiguration _configuration;

    public AiNeighborhoodAnalysisService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(
        GenerateNeighborhoodAiAnalysisRequest request)
    {
        try
        {
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

            return new BaseResponse<NeighborhoodAiAnalysisResponse>
            {
                Success = true,
                Message = "AI mahalle analizi başarıyla oluşturuldu.",
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