using Muhit.Domain.Common;

namespace Muhit.Domain.Entities
{
    public class NeighborhoodReview : BaseEntity
    {
        public int AppUserId { get; set; }
        public int NeighborhoodId { get; set; }
        public int SafetyScore { get; set; }
        public int TransportScore { get; set; }
        public int QuietnessScore { get; set; }
        public int SocialLifeScore { get; set; }
        public int CostScore { get; set; }
        public string? Comment { get; set; }
        public Neighborhood Neighborhood { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
    }
}
