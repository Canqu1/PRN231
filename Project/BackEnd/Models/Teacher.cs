using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class Teacher
    {
        public Teacher()
        {
            Evaluations = new HashSet<Evaluation>();
        }

        public int TeacherId { get; set; }
        public int AccountId { get; set; }
        public string TeacherName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? HireDate { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<Evaluation> Evaluations { get; set; }
    }
}
