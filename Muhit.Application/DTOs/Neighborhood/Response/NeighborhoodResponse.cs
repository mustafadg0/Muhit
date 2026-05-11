using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
    }
}
