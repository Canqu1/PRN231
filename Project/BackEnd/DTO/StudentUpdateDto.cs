namespace BackEnd.DTO
{
    public class StudentUpdateDto
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }
        public string Address { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
