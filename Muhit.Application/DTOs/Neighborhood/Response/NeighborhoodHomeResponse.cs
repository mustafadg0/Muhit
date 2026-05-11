using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodHomeResponse
    {
        public int NeighborhoodId { get; set; }
        public string NeighborhoodName { get; set; } = null!;
        public int DistrictId { get; set; }
        public string DistrictName { get; set; } = null!;
        public int CityId { get; set; }
        public string CityName { get; set; } = null!;
        public int ReviewCount { get; set; }
        public decimal AverageScore { get; set; }
        public List<NeighborhoodReviewItemResponse> UserReviews { get; set; } = new();
        public NeighborhoodAiAnalysisResponse? AiAnalysis { get; set; }
    }
}
