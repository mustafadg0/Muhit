using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services;

public class NeighborhoodService : INeighborhoodService
{
    private readonly MuhitDbContext _context;

    public NeighborhoodService(MuhitDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<List<NeighborhoodResponse>>> GetAllAsync()
    {
        var response = new BaseResponse<List<NeighborhoodResponse>>();

        var neighborhoods = await Query()
            .ToListAsync();

        response.Success = true;
        response.Message = "Mahalleler başarıyla getirildi.";
        response.Data = neighborhoods;

        return response;
    }

    public async Task<BaseResponse<NeighborhoodResponse>> GetByIdAsync(int id)
    {
        var response = new BaseResponse<NeighborhoodResponse>();

        var neighborhood = await Query()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (neighborhood == null)
        {
            response.Success = false;
            response.Message = "Mahalle bulunamadı.";

            return response;
        }

        response.Success = true;
        response.Message = "Mahalle başarıyla getirildi.";
        response.Data = neighborhood;

        return response;
    }

    public async Task<BaseResponse<List<NeighborhoodResponse>>> GetByDistrictIdAsync(int districtId)
    {
        var response = new BaseResponse<List<NeighborhoodResponse>>();

        var neighborhoods = await Query()
            .Where(x => x.DistrictId == districtId)
            .ToListAsync();

        response.Success = true;
        response.Message = "Mahalleler başarıyla getirildi.";
        response.Data = neighborhoods;

        return response;
    }

    public async Task<BaseResponse<List<NeighborhoodResponse>>> SearchAsync(string keyword)
    {
        var response = new BaseResponse<List<NeighborhoodResponse>>();

        keyword = keyword.Trim().ToLower();

        var neighborhoods = await Query()
            .Where(x =>
                x.Name.ToLower().Contains(keyword) ||
                x.DistrictName.ToLower().Contains(keyword) ||
                x.CityName.ToLower().Contains(keyword))
            .ToListAsync();

        response.Success = true;
        response.Message = "Arama başarılı.";
        response.Data = neighborhoods;

        return response;
    }

    private IQueryable<NeighborhoodResponse> Query()
    {
        return _context.Neighborhoods
            .AsNoTracking()
            .Include(x => x.District)
            .ThenInclude(x => x.City)
            .Select(x => new NeighborhoodResponse
            {
                Id = x.Id,
                Name = x.Name,
                DistrictId = x.DistrictId,
                DistrictName = x.District.Name,
                CityId = x.District.CityId,
                CityName = x.District.City.Name
            });
    }
}