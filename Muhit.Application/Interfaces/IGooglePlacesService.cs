using Muhit.Application.DTOs.Google;

namespace Muhit.Application.Interfaces;

public interface IGooglePlacesService
{
    Task<GoogleAmenityCountResult> GetAmenitySummaryAsync(
        double latitude,
        double longitude,
        int radiusMeters);
}