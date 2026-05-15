using Muhit.Domain.Common;

public class NeighborhoodAmenitySummary : BaseEntity
{
    public int NeighborhoodId { get; set; }
    public Neighborhood Neighborhood { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int RadiusMeters { get; set; }
    public int CafeCount { get; set; }
    public int RestaurantCount { get; set; }
    public int MarketCount { get; set; }
    public int HospitalCount { get; set; }
    public int PharmacyCount { get; set; }
    public int SchoolCount { get; set; }
    public int ParkCount { get; set; }
    public int GymCount { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}