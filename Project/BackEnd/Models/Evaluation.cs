using System;
using System.Collections.Generic;

namespace BackEnd.Models
{
    public partial class Evaluation
    {
        public int EvaluationId { get; set; }
        public int Grade { get; set; }
        public string AdditionExplanation { get; set; } = null!;
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }

        public virtual Student Student { get; set; } = null!;
        public virtual Teacher? Teacher { get; set; }
    }
}
