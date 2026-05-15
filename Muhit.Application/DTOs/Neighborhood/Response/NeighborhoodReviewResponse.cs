public class NeighborhoodReviewResponse
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public int SafetyScore { get; set; }
    public int TransportScore { get; set; }
    public int QuietnessScore { get; set; }
    public int SocialLifeScore { get; set; }
    public int CostScore { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedDate { get; set; }
}