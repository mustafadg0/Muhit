using Muhit.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Domain.Entities
{
    public class District : BaseEntity
    {
        public int CityId { get; set; }
        public string Name { get; set; } = null!;
        public City City { get; set; } = null!;
    }
}
