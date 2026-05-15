using Muhit.Domain.Common;
using Muhit.Domain.Entities;

public class NeighborhoodReview : BaseEntity
{
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public int? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public int SafetyScore { get; set; }
    public int TransportScore { get; set; }
    public int QuietnessScore { get; set; }
    public int SocialLifeScore { get; set; }
    public int CostScore { get; set; }
    public string? Comment { get; set; }
}