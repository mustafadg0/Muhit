using Muhit.Domain.Common;
using Muhit.Domain.Entities;

public class Neighborhood : BaseEntity
{
    public string Name { get; set; }

    public int DistrictId { get; set; }
    public District District { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ICollection<NeighborhoodReview> Reviews { get; set; } = new List<NeighborhoodReview>();
    public ICollection<AppUser> Residents { get; set; } = new List<AppUser>();
    public NeighborhoodAmenitySummary? AmenitySummary { get; set; }
    public NeighborhoodAiAnalysis? AiAnalysis { get; set; }
}