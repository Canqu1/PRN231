namespace BackEnd.DTO
{
    public class CreateTeacherDTO
    {
        public int? TeacherId { get; set; }
        public int? AccountId { get; set; }
        public string? TeacherName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? HireDate { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }
}
