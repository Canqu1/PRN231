using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Students = new HashSet<Student>();
        }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;

        public virtual ICollection<Student> Students { get; set; }
    }
}
