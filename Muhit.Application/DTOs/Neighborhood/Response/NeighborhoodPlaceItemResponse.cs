using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodPlaceItemResponse
    {
        public int Id { get; set; }
        public string GooglePlaceId { get; set; }
        public string Name { get; set; }
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal? Rating { get; set; }
        public int? UserRatingCount { get; set; }
        public string PrimaryType { get; set; }
    }
}
