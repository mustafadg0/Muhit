using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodAiAnalysisResponse
    {
        public string Summary { get; set; } = null!;

        public NeighborhoodAiCategoryScoreResponse Safety { get; set; } = null!;

        public NeighborhoodAiCategoryScoreResponse Transport { get; set; } = null!;

        public NeighborhoodAiCategoryScoreResponse Quietness { get; set; } = null!;

        public NeighborhoodAiCategoryScoreResponse SocialLife { get; set; } = null!;

        public NeighborhoodAiCategoryScoreResponse Cost { get; set; } = null!;

        public List<string> BestFor { get; set; } = new();

        public List<string> NotIdealFor { get; set; } = new();
    }
}
