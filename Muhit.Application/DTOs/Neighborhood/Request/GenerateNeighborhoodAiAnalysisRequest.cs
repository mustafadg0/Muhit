using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Request
{
    public class GenerateNeighborhoodAiAnalysisRequest
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;

        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;

        public int NeighborhoodId { get; set; }
        public string NeighborhoodName { get; set; } = null!;
    }
}
