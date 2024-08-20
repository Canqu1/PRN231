using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class Account
    {
        public Account()
        {
            Students = new HashSet<Student>();
        }

        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? ActiveCode { get; set; }
        public bool? IsActive { get; set; }

        public virtual Teacher? Teacher { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
