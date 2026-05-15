using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodReviewItemResponse
    {
        public int Id { get; set; }
        public int? AppUserId { get; set; }
        public string UserFullName { get; set; } = null!;
        public string? Comment { get; set; }
        public int SafetyScore { get; set; }
        public int TransportScore { get; set; }
        public int QuietnessScore { get; set; }
        public int SocialLifeScore { get; set; }
        public int CostScore { get; set; }
        public decimal AverageScore =>
            Math.Round(
                (decimal)(
                    SafetyScore +
                    TransportScore +
                    QuietnessScore +
                    SocialLifeScore +
                    CostScore
                ) / 5,
                1);
        public DateTime CreatedDate { get; set; }
    }
}
