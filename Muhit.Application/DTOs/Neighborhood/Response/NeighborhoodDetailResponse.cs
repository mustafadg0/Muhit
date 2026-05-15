using Muhit.Application.DTOs.Neighborhood.Response;

public class NeighborhoodDetailResponse
{
    public int NeighborhoodId { get; set; }

    public string City { get; set; }
    public string District { get; set; }
    public string Neighborhood { get; set; }

    public string? Summary { get; set; }

    public NeighborhoodUserReviewSummaryResponse UserReviewSummary { get; set; }
    public List<NeighborhoodReviewResponse> UserReviews { get; set; }

    public NeighborhoodAiAnalysisResponse? AiAnalysis { get; set; }

    public NeighborhoodAmenitySummaryResponse Amenities { get; set; }
}