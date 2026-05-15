using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.City.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services
{
    public class CityService : ICityService
    {
        private readonly MuhitDbContext _context;
        public CityService(MuhitDbContext context)
        {
            _context = context;
        }
        public async Task<BaseResponse<List<CityResponse>>> GetAllAsync()
        {
            var response = new BaseResponse<List<CityResponse>>();
            var cities = await _context.Cities.Select(x => new CityResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
            response.Success = true;
            response.Message = "Ýller baþarýyla getirildi.";
            response.Data = cities;
            return response;
        }
    }
}
