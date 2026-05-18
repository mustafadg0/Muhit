using Muhit.Application.DTOs.Google;
using Muhit.Application.DTOs.Google.Response;

namespace Muhit.Application.Interfaces;

public interface IGooglePlacesService
{
    Task<GoogleAmenityCountResult> GetAmenitySummaryAsync(
        double latitude,
        double longitude,
        int radiusMeters);
    Task<List<GooglePlace>> GetTopPlacesAsync(
        double latitude,
        double longitude,
        int radiusMeters,
        string placeType);
}