using Muhit.Application.DTOs.Neighborhood.Response;

public class NeighborhoodAiAnalysisResponse
{
    public string Summary { get; set; }
    public NeighborhoodAiCategoryScoreResponse Safety { get; set; }
    public NeighborhoodAiCategoryScoreResponse Transport { get; set; }
    public NeighborhoodAiCategoryScoreResponse Quietness { get; set; }
    public NeighborhoodAiCategoryScoreResponse SocialLife { get; set; }
    public NeighborhoodAiCategoryScoreResponse Cost { get; set; }
    public List<string> BestFor { get; set; }
    public List<string> NotIdealFor { get; set; }
}