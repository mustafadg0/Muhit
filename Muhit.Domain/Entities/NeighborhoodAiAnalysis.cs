using Muhit.Domain.Common;

public class NeighborhoodAiAnalysis : BaseEntity
{
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public string Summary { get; set; }
    public int SafetyScore { get; set; }
    public string SafetyComment { get; set; }
    public int TransportScore { get; set; }
    public string TransportComment { get; set; }
    public int QuietnessScore { get; set; }
    public string QuietnessComment { get; set; }
    public int SocialLifeScore { get; set; }
    public string SocialLifeComment { get; set; }
    public int CostScore { get; set; }
    public string CostComment { get; set; }

    public string BestForJson { get; set; }
    public string NotIdealForJson { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}