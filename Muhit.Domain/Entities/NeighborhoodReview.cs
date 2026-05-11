using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Domain.Entities
{
    public class NeighborhoodReview
    {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public int NeighborhoodId { get; set; }
        public int SafetyScore { get; set; }
        public int TransportScore { get; set; }
        public int QuietnessScore { get; set; }
        public int SocialLifeScore { get; set; }
        public int CostScore { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Neighborhood Neighborhood { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}
