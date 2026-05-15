using Muhit.Application.Common;
using Muhit.Application.DTOs.City.Response;

namespace Muhit.Application.Interfaces
{
    public interface ICityService
    {
        Task<BaseResponse<List<CityResponse>>> GetAllAsync();
    }
}
