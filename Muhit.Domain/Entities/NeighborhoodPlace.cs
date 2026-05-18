using Muhit.Domain.Common;

public class NeighborhoodPlace : BaseEntity
{
    public int NeighborhoodId { get; set; }

    public string GooglePlaceId { get; set; }
    public string Name { get; set; }
    public string FormattedAddress { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public decimal? Rating { get; set; }
    public int? UserRatingCount { get; set; }

    public string PrimaryType { get; set; } // restaurant, cafe
    public string TypesJson { get; set; } // tüm types array json

    public DateTime LastFetchedAt { get; set; }
}