using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Request;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;

namespace Muhit.Infrastructure.Services;

public class AiNeighborhoodAnalysisService : IAiNeighborhoodAnalysisService
{
    public async Task<BaseResponse<NeighborhoodAiAnalysisResponse>> GenerateAsync(
        GenerateNeighborhoodAiAnalysisRequest request)
    {
        await Task.CompletedTask;

        return new BaseResponse<NeighborhoodAiAnalysisResponse>
        {
            Success = true,
            Message = "AI mahalle analizi başarıyla oluşturuldu.",
            Data = new NeighborhoodAiAnalysisResponse
            {
                Summary =
            $"{request.CityName} / {request.DistrictName} / {request.NeighborhoodName} için genel muhit analizi hazırlandı.",

                Safety = new NeighborhoodAiCategoryScoreResponse
                {
                    Score = 3,
                    Comment = "Bölge genel olarak düşük seviyede güvenli kabul edilmektedir. (Yani çaqqal dolu)"
                },

                Transport = new NeighborhoodAiCategoryScoreResponse
                {
                    Score = 4,
                    Comment = "Toplu taşıma erişimi güçlüdür."
                },

                Quietness = new NeighborhoodAiCategoryScoreResponse
                {
                    Score = 3,
                    Comment = "Bölge zaman zaman yoğun ve hareketli olabilir."
                },

                SocialLife = new NeighborhoodAiCategoryScoreResponse
                {
                    Score = 4,
                    Comment = "Kafe, restoran ve sosyal alanlar açısından güçlüdür."
                },

                Cost = new NeighborhoodAiCategoryScoreResponse
                {
                    Score = 3,
                    Comment = "Yaşam maliyeti orta seviyededir."
                },

                BestFor = new List<string>
                {
                    "Genç profesyoneller",
                    "Öğrenciler"
                },

                NotIdealFor = new List<string>
                {
                    "Sessizlik arayanlar"
                }
            }
        };
    }
}