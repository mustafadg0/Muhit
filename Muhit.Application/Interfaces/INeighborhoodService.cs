using Muhit.Application.Common;
using Muhit.Application.DTOs.Neighborhood.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.Interfaces
{
    public interface INeighborhoodService
    {
        Task<BaseResponse<List<NeighborhoodResponse>>> GetAllAsync();
        Task<BaseResponse<NeighborhoodResponse>> GetByIdAsync(int id);
        Task<BaseResponse<List<NeighborhoodResponse>>> GetByDistrictIdAsync(int districtId);
        Task<BaseResponse<List<NeighborhoodResponse>>> SearchAsync(string keyword);
    }
}
