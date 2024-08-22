namespace BackEnd.DTO
{
    public class StudentSubjectsDTO
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; } = null!;
        public List<SubjectListDTO> Subjects { get; set; } = new List<SubjectListDTO>();
    }

    public class SubjectListDTO
    {
        public string SubjectName { get; set; } = null!;
        public int Grade { get; set; }
        public string AdditionExplanation { get; set; } = null!;
        public string TeacherName { get; set; } = null!;
    }
}
