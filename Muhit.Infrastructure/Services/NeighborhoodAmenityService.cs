using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Muhit.Application.Common;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodAmenityService : INeighborhoodAmenityService
{
    private readonly MuhitDbContext _context;
    private readonly IGoogleGeocodingService _geocodingService;
    private readonly IGooglePlacesService _placesService;
    private readonly IConfiguration _configuration;

    public NeighborhoodAmenityService(
        MuhitDbContext context,
        IGoogleGeocodingService geocodingService,
        IGooglePlacesService placesService,
        IConfiguration configuration)
    {
        _context = context;
        _geocodingService = geocodingService;
        _placesService = placesService;
        _configuration = configuration;
    }

    private static int GetIntConfigValue(
        IConfiguration configuration,
        string key,
        int defaultValue)
    {
        var value = configuration.GetSection(key).Value;

        return int.TryParse(value, out var result)
            ? result
            : defaultValue;
    }

    public async Task<BaseResponse<NeighborhoodAmenitySummaryResponse>>
        GetOrCreateSummaryAsync(int neighborhoodId)
    {
        try
        {
            var cacheDays = 7;

            var radiusMeters = GetIntConfigValue(
                _configuration,
                "GoogleMaps:DefaultRadiusMeters",
                1500);

            var neighborhood = await _context.Neighborhoods
                .Include(x => x.District)
                    .ThenInclude(x => x.City)
                .Include(x => x.AmenitySummary)
                .FirstOrDefaultAsync(x => x.Id == neighborhoodId);

            if (neighborhood == null)
            {
                return new BaseResponse<NeighborhoodAmenitySummaryResponse>
                {
                    Success = false,
                    Message = "Mahalle bulunamadı."
                };
            }

            var summary = neighborhood.AmenitySummary;

            if (summary != null &&
                summary.LastUpdatedAt >= DateTime.UtcNow.AddDays(-cacheDays))
            {
                return new BaseResponse<NeighborhoodAmenitySummaryResponse>
                {
                    Success = true,
                    Message = "Mahalle çevre bilgileri cache üzerinden getirildi.",
                    Data = MapAmenity(summary)
                };
            }

            double latitude;
            double longitude;

            if (neighborhood.Latitude.HasValue &&
                neighborhood.Longitude.HasValue)
            {
                latitude = neighborhood.Latitude.Value;
                longitude = neighborhood.Longitude.Value;
            }
            else
            {
                var address =
                    $"{neighborhood.Name} Mahallesi, " +
                    $"{neighborhood.District.Name}, " +
                    $"{neighborhood.District.City.Name}, Türkiye";

                var location = await _geocodingService
                    .GetLocationAsync(address);

                latitude = location.Latitude;
                longitude = location.Longitude;

                neighborhood.Latitude = latitude;
                neighborhood.Longitude = longitude;
            }

            var googleResult = await _placesService
                .GetAmenitySummaryAsync(
                    latitude,
                    longitude,
                    radiusMeters);

            if (summary == null)
            {
                summary = new NeighborhoodAmenitySummary
                {
                    NeighborhoodId = neighborhood.Id
                };

                await _context
                    .NeighborhoodAmenitySummaries
                    .AddAsync(summary);
            }

            summary.Latitude = latitude;
            summary.Longitude = longitude;
            summary.RadiusMeters = radiusMeters;

            summary.CafeCount = googleResult.CafeCount;
            summary.RestaurantCount = googleResult.RestaurantCount;
            summary.MarketCount = googleResult.MarketCount;
            summary.HospitalCount = googleResult.HospitalCount;
            summary.PharmacyCount = googleResult.PharmacyCount;
            summary.SchoolCount = googleResult.SchoolCount;
            summary.ParkCount = googleResult.ParkCount;
            summary.GymCount = googleResult.GymCount;

            summary.LastUpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BaseResponse<NeighborhoodAmenitySummaryResponse>
            {
                Success = true,
                Message = "Mahalle çevre bilgileri başarıyla oluşturuldu.",
                Data = MapAmenity(summary)
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<NeighborhoodAmenitySummaryResponse>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    private static NeighborhoodAmenitySummaryResponse MapAmenity(
        NeighborhoodAmenitySummary summary)
    {
        return new NeighborhoodAmenitySummaryResponse
        {
            CafeCount = summary.CafeCount,
            RestaurantCount = summary.RestaurantCount,
            MarketCount = summary.MarketCount,
            HospitalCount = summary.HospitalCount,
            PharmacyCount = summary.PharmacyCount,
            SchoolCount = summary.SchoolCount,
            ParkCount = summary.ParkCount,
            GymCount = summary.GymCount,
            RadiusMeters = summary.RadiusMeters,
            LastUpdatedAt = summary.LastUpdatedAt
        };
    }
}