using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.Neighborhood.Response
{
    public class NeighborhoodAiCategoryScoreResponse
    {
        public double Score { get; set; }
        public string Comment { get; set; } = null!;
    }
}
