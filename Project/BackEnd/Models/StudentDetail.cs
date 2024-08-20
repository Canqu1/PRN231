using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class StudentDetail
    {
        public int StudentDetailsId { get; set; }
        public string Address { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public int StudentId { get; set; }

        public virtual Student Student { get; set; } = null!;
    }
}
