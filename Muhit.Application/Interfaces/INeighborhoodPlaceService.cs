using Muhit.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.Interfaces
{
    public interface INeighborhoodPlaceService
    {
        Task<BaseResponse<bool>> RefreshNeighborhoodPlacesAsync(int neighborhoodId);
    }
}
