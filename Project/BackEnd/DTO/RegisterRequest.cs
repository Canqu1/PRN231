namespace BackEnd.DTO
{
    public class RegisterRequest
    {
          public  string Email { get; set; }
          public  string Password { get; set; }
         public   string Name { get; set; }
         public   string Address { get; set; }
         public   string Phone { get; set; }
         public  string AdditionalInformation { get; set; }
           public int Age { get; set; }
          public  bool isRegular { get; set; }

    }
}
