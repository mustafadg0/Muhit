using Muhit.Domain.Common;

public class NeighborhoodAiAnalysis : BaseEntity
{
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public string Summary { get; set; }
    public double SafetyScore { get; set; }
    public string SafetyComment { get; set; }
    public double TransportScore { get; set; }
    public string TransportComment { get; set; }
    public double QuietnessScore { get; set; }
    public string QuietnessComment { get; set; }
    public double SocialLifeScore { get; set; }
    public string SocialLifeComment { get; set; }
    public double CostScore { get; set; }
    public string CostComment { get; set; }

    public string BestForJson { get; set; }
    public string NotIdealForJson { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}