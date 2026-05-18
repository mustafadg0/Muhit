using Microsoft.Extensions.Configuration;
using Muhit.Application.DTOs.Google;
using Muhit.Application.DTOs.Google.Response;
using Muhit.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Muhit.Infrastructure.Services;

public class GooglePlacesService : IGooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GooglePlacesService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<GoogleAmenityCountResult> GetAmenitySummaryAsync(
        double latitude,
        double longitude,
        int radiusMeters)
    {
        return new GoogleAmenityCountResult
        {
            CafeCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "cafe"),
            RestaurantCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "restaurant"),
            MarketCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "supermarket"),
            HospitalCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "hospital"),
            PharmacyCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "pharmacy"),
            SchoolCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "school"),
            ParkCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "park"),
            GymCount = await GetPlaceCountAsync(latitude, longitude, radiusMeters, "gym")
        };
    }

    private async Task<int> GetPlaceCountAsync(
        double latitude,
        double longitude,
        int radiusMeters,
        string placeType)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        var body = new
        {
            includedTypes = new[] { placeType },
            maxResultCount = 20,
            locationRestriction = new
            {
                circle = new
                {
                    center = new
                    {
                        latitude,
                        longitude
                    },
                    radius = radiusMeters
                }
            },
            languageCode = "tr",
            regionCode = "TR"
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://places.googleapis.com/v1/places:searchNearby");

        request.Headers.Add("X-Goog-Api-Key", apiKey);
        request.Headers.Add("X-Goog-FieldMask", "places.id");

        request.Content = JsonContent.Create(body);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return 0;

        var result = await response
            .Content
            .ReadFromJsonAsync<GoogleNearbySearchResponse>();

        return result?.Places?.Count ?? 0;
    }

    public async Task<List<GooglePlace>> GetTopPlacesAsync(
    double latitude,
    double longitude,
    int radiusMeters,
    string placeType)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        var body = new
        {
            includedTypes = new[] { placeType },
            maxResultCount = 20,
            rankPreference = "POPULARITY",
            locationRestriction = new
            {
                circle = new
                {
                    center = new
                    {
                        latitude,
                        longitude
                    },
                    radius = radiusMeters
                }
            },
            languageCode = "tr",
            regionCode = "TR"
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://places.googleapis.com/v1/places:searchNearby");

        request.Headers.Add("X-Goog-Api-Key", apiKey);
        request.Headers.Add(
            "X-Goog-FieldMask",
            "places.id,places.types,places.formattedAddress,places.location,places.rating,places.userRatingCount,places.displayName");

        request.Content = JsonContent.Create(body);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new List<GooglePlace>();

        var result = await response.Content.ReadFromJsonAsync<GoogleNearbySearchResponse>();

        return result?.Places ?? new List<GooglePlace>();
    }
}
