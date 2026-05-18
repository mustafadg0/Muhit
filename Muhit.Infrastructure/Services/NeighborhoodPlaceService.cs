using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Google.Response;
using Muhit.Application.Interfaces;
using Muhit.Domain.Entities;
using Muhit.Persistence.Context;
using System.Text.Json;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodPlaceService : INeighborhoodPlaceService
{
    private readonly MuhitDbContext _context;
    private readonly IGooglePlacesService _googlePlacesService;
    private readonly IGoogleGeocodingService _googleGeocodingService;

    public NeighborhoodPlaceService(
        MuhitDbContext context,
        IGooglePlacesService googlePlacesService,
        IGoogleGeocodingService googleGeocodingService)
    {
        _context = context;
        _googlePlacesService = googlePlacesService;
        _googleGeocodingService = googleGeocodingService;
    }

    public async Task<BaseResponse<bool>> RefreshNeighborhoodPlacesAsync(int neighborhoodId)
    {
        var neighborhood = await _context.Neighborhoods
            .Include(x => x.District)
            .ThenInclude(x => x.City)
            .FirstOrDefaultAsync(x =>
                x.Id == neighborhoodId &&
                x.IsActive &&
                !x.IsDeleted);

        if (neighborhood == null)
        {
            return new BaseResponse<bool>
            {
                Success = false,
                Message = "Mahalle bulunamadı.",
                Data = false
            };
        }

        var address = $"{neighborhood.Name}, {neighborhood.District.Name}, {neighborhood.District.City.Name}";

        var location = await _googleGeocodingService.GetLocationAsync(address);

        var restaurantPlaces = await _googlePlacesService.GetTopPlacesAsync(
            location.Latitude,
            location.Longitude,
            1500,
            "restaurant");

        var cafePlaces = await _googlePlacesService.GetTopPlacesAsync(
            location.Latitude,
            location.Longitude,
            1500,
            "cafe");

        await UpsertPlacesAsync(neighborhoodId, restaurantPlaces, "restaurant");
        await UpsertPlacesAsync(neighborhoodId, cafePlaces, "cafe");

        await _context.SaveChangesAsync();

        return new BaseResponse<bool>
        {
            Success = true,
            Message = "Mahalle restoran ve cafe verileri güncellendi.",
            Data = true
        };
    }

    private async Task UpsertPlacesAsync(
        int neighborhoodId,
        List<GooglePlace> places,
        string primaryType)
    {
        foreach (var place in places)
        {
            if (string.IsNullOrWhiteSpace(place.Id))
                continue;

            var existingPlace = await _context.NeighborhoodPlaces
                .FirstOrDefaultAsync(x =>
                    x.NeighborhoodId == neighborhoodId &&
                    x.GooglePlaceId == place.Id);

            if (existingPlace == null)
            {
                var entity = new NeighborhoodPlace
                {
                    NeighborhoodId = neighborhoodId,
                    GooglePlaceId = place.Id,
                    Name = place.DisplayName?.Text,
                    FormattedAddress = place.FormattedAddress,
                    Latitude = place.Location.Latitude,
                    Longitude = place.Location.Longitude,
                    Rating = place.Rating.HasValue ? Convert.ToDecimal(place.Rating.Value) : null,
                    UserRatingCount = place.UserRatingCount,
                    PrimaryType = primaryType,
                    TypesJson = JsonSerializer.Serialize(place.Types),
                    LastFetchedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.NeighborhoodPlaces.AddAsync(entity);
            }
            else
            {
                existingPlace.Name = place.DisplayName?.Text;
                existingPlace.FormattedAddress = place.FormattedAddress;
                existingPlace.Latitude = place.Location.Latitude;
                existingPlace.Longitude = place.Location.Longitude;
                existingPlace.Rating = place.Rating.HasValue ? Convert.ToDecimal(place.Rating.Value) : null;
                existingPlace.UserRatingCount = place.UserRatingCount;
                existingPlace.PrimaryType = primaryType;
                existingPlace.TypesJson = JsonSerializer.Serialize(place.Types);
                existingPlace.LastFetchedAt = DateTime.UtcNow;
                existingPlace.UpdatedDate = DateTime.UtcNow;
            }
        }
    }
}