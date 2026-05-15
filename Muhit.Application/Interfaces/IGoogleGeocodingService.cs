using Muhit.Application.DTOs.Google.Response;

namespace Muhit.Application.Interfaces;

public interface IGoogleGeocodingService
{
    Task<GoogleLocationResponse> GetLocationAsync(string address);
}