namespace BackEnd.DTO
{
    public class StudentRep
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public List<EvaluationDTO> Evaluation { get; set; }

    }
    public class StudentRepDTO
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public List<SubjectDTO> Subjects { get; set; }
    }
}
