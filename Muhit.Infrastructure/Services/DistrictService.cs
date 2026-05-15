using Microsoft.EntityFrameworkCore;
using Muhit.Application.Common;
using Muhit.Application.DTOs.District.Response;
using Muhit.Application.Interfaces;
using Muhit.Persistence.Context;

namespace Muhit.Infrastructure.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly MuhitDbContext _context;
        public DistrictService(MuhitDbContext context)
        {
            _context = context;
        }
        public async Task<BaseResponse<List<DistrictResponse>>> GetByCityIdAsync(int cityId)
        {
            var response = new BaseResponse<List<DistrictResponse>>();
            var districts = await _context.Districts
                .Where(x => x.CityId == cityId)
                .Select(x => new DistrictResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    CityId = x.CityId
                }).ToListAsync();
            response.Success = true;
            response.Message = "Ýlçeler baþarýyla getirildi.";
            response.Data = districts;
            return response;
        }
    }
}
