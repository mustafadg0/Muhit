using Muhit.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Domain.Entities
{
    public class Neighborhood : BaseEntity
    {
        public int DistrictId { get; set; }
        public string Name { get; set; } = null!;
        public District District { get; set; } = null!;
        public ICollection<AppUser> Residents { get; set; } = new List<AppUser>();
        public ICollection<NeighborhoodReview> Reviews { get; set; } = new List<NeighborhoodReview>();
    }
}
