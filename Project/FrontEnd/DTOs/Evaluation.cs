namespace FrontEnd.DTOs
{
    public class Evaluation
    {
        public int? EvaluationId { get; set; }
        public int? Grade { get; set; }
        public string? AdditionExplanation { get; set; } = null!;
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
    }
}
