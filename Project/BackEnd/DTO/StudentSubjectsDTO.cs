namespace BackEnd.DTO
{
    public class StudentSubjectsDTO
    {
        public string StudentName { get; set; }
        public List<SubjectListDTO> Subjects { get; set; } = new List<SubjectListDTO>();
    }
    public class SubjectListDTO
    {
        public string SubjectName { get; set; } = null!;
        public List<EvaluationsDTO> Evaluations { get; set; } = new List<EvaluationsDTO>();
    }

    public class EvaluationsDTO
    {
        public int Grade { get; set; }
        public string AdditionExplanation { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
    }
}
