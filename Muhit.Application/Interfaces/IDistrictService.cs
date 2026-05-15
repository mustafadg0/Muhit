using Muhit.Application.Common;
using Muhit.Application.DTOs.District.Response;

namespace Muhit.Application.Interfaces
{
    public interface IDistrictService
    {
        Task<BaseResponse<List<DistrictResponse>>> GetByCityIdAsync(int cityId);
    }
}
