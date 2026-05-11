namespace Muhit.Domain.Entities;

public class NeighborhoodAiAnalysis
{
    public int Id { get; set; }
    public int NeighborhoodId { get; set; }
    public string Summary { get; set; } = null!;
    public int SafetyScore { get; set; }
    public int TransportScore { get; set; }
    public int QuietnessScore { get; set; }
    public int SocialLifeScore { get; set; }
    public int CostScore { get; set; }
    public string? BestFor { get; set; }
    public string? NotIdealFor { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedDate { get; set; }
    public Neighborhood Neighborhood { get; set; } = null!;
}