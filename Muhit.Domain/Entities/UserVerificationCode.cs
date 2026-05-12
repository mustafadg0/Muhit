using Muhit.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muhit.Domain.Entities
{
    public class UserVerificationCode : BaseEntity
    {
        public int AppUserId { get; set; }
        public string Code { get; set; } = null!;
        public string VerificationType { get; set; } = null!; // Phone, Email, TwoFactor
        public DateTime ExpireDate { get; set; }
        public bool IsUsed { get; set; }
        public AppUser AppUser { get; set; } = null!;
    }
}
