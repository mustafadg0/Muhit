using Microsoft.Extensions.Configuration;
using Muhit.Application.DTOs.Google;
using Muhit.Application.DTOs.Google.Response;
using Muhit.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Muhit.Infrastructure.Services;

public class GoogleGeocodingService : IGoogleGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleGeocodingService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<GoogleLocationResponse> GetLocationAsync(string address)
    {
        var apiKey = _configuration["GoogleMaps:ApiKey"];

        var url =
            "https://maps.googleapis.com/maps/api/geocode/json" +
            $"?address={Uri.EscapeDataString(address)}" +
            $"&region=tr" +
            $"&language=tr" +
            $"&key={apiKey}";

        var response = await _httpClient
            .GetFromJsonAsync<GoogleGeocodeResponse>(url);

        if (response == null || response.Status != "OK")
            throw new Exception($"Google Geocoding başarısız: {response?.Status}");

        var location = response.Results
            .FirstOrDefault()?
            .Geometry?
            .Location;

        if (location == null)
            throw new Exception("Konum bulunamadı.");

        return new GoogleLocationResponse
        {
            Latitude = location.Lat,
            Longitude = location.Lng
        };
    }
}

public class GoogleGeocodeResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("results")]
    public List<GoogleGeocodeResult> Results { get; set; } = new();
}

public class GoogleGeocodeResult
{
    [JsonPropertyName("geometry")]
    public GoogleGeometry Geometry { get; set; }
}

public class GoogleGeometry
{
    [JsonPropertyName("location")]
    public GoogleLatLng Location { get; set; }
}

public class GoogleLatLng
{
    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lng")]
    public double Lng { get; set; }
}