namespace BackEnd.DTO
{
    public class EvaluationDTO
    {
        public int EvaluationId { get; set; }
        public int Grade { get; set; }
        public string AdditionExplanation { get; set; } = null!;
    }
}
