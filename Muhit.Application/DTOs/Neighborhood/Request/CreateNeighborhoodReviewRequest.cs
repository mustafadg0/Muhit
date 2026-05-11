namespace Muhit.Application.DTOs.NeighborhoodReview.Request;

public class CreateNeighborhoodReviewRequest
{
    public int NeighborhoodId { get; set; }
    public int AppUserId { get; set; }
    public int SafetyScore { get; set; }
    public int TransportScore { get; set; }
    public int QuietnessScore { get; set; }
    public int SocialLifeScore { get; set; }
    public int CostScore { get; set; }
    public string? Comment { get; set; }
}