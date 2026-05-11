using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Application.DTOs.User.Request
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int? CurrentNeighborhoodId { get; set; }
    }
}
