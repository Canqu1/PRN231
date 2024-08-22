namespace FrontEnd.Models
{
    public class SubjectDTO
    {
        public string SubjectName { get; set; }
    }

    public class SubjectReqDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
    public class UpdateSubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }
}
