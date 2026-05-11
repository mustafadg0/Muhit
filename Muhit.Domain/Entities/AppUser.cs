using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Domain.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsEmailVerified { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsPhoneNumberVerified { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public int? CurrentNeighborhoodId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public Neighborhood? CurrentNeighborhood { get; set; }
        public ICollection<NeighborhoodReview> Reviews { get; set; } = new List<NeighborhoodReview>();
    }
}
