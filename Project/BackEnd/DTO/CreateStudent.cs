namespace BackEnd.DTO
{
    public class CreateStudent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsRegularStudent { get; set; }

        public string Address { get; set; }

        public string AdditionalInformation { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
    }
}
