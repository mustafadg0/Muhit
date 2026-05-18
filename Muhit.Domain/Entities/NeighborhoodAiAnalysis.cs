using Muhit.Domain.Common;

public class NeighborhoodAiAnalysis : BaseEntity
{
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public string Summary { get; set; }
    public decimal SafetyScore { get; set; }
    public string SafetyComment { get; set; }
    public decimal TransportScore { get; set; }
    public string TransportComment { get; set; }
    public decimal QuietnessScore { get; set; }
    public string QuietnessComment { get; set; }
    public decimal SocialLifeScore { get; set; }
    public string SocialLifeComment { get; set; }
    public decimal CostScore { get; set; }
    public string CostComment { get; set; }
    public string BestForJson { get; set; }
    public string NotIdealForJson { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}