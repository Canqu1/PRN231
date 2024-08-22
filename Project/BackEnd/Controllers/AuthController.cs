using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using MimeKit;

using System.Net;
using System.Security.Claims;
using System.Text;
using BackEnd.DTO;


namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;
        private readonly IConfiguration _configuration;

        public AuthController(ProjectPRN231Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string CreateToken()
        {
            var key = new SymmetricSecurityKey(
              System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                 expires: DateTime.Now.AddDays(1),
                 signingCredentials: creds);
          return  new JwtSecurityTokenHandler().WriteToken(token);
        }
        // POST api/<AuthController>
        [HttpPost("Login")]
        public IActionResult Login(LoginRequest loginRequest)
        {

            var byteData = Encoding.UTF8.GetBytes(loginRequest.Password);
            var encryptData = Convert.ToBase64String(byteData);
            Account? account = _context.Accounts.FirstOrDefault(x => x.Email == loginRequest.Email && x.Password == encryptData);
            var name = "";
            if (account.Type.ToLower() == "teacher")
            {
                Teacher teacher = _context.Teachers.FirstOrDefault(x => x.AccountId == account.AccountId);
                name = teacher.TeacherName;
            }
            else if (account.Type.ToLower() == "student")
            {
                Student? student = _context.Students.FirstOrDefault(x => x.AccountId == account.AccountId);
                name = student.Name;
            }
            else if (account.Type.ToLower() == "admin")
            {
                name = "Admin";
            }
            if (account == null)
            {
                return Unauthorized();
            }
          
            var claims = new Claim[]
          {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("role", account.Type.ToString() ),
                new Claim("uid", account.AccountId.ToString()),
                new Claim("name", name),
          };
            var key = new SymmetricSecurityKey(
             System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("Register")]
        public   async Task<IActionResult>  Register(RegisterRequest registerRequest)
        {
            var byteData = Encoding.UTF8.GetBytes(registerRequest.Password);
            var encryptData = Convert.ToBase64String(byteData);
            //var account = _context.Accounts.FirstOrDefault(x => x.Email == registerRequest.Email);
            //if (account != null)
            //{
            //    return BadRequest();
            //}
        
            Guid guidActiveCode = Guid.NewGuid();
            Account accounts = new Account()
            {
                Email = registerRequest.Email,
                Password = encryptData,
                Type = "student",
                ActiveCode = guidActiveCode.ToString().Trim(),
                IsActive = false
            };
             _context.Accounts.Add(accounts);
            await _context.SaveChangesAsync();

            Student student = new Student()
            {
                AccountId = accounts.AccountId,
                Name = registerRequest.Name,
                Age = registerRequest.Age,
                IsRegularStudent = /*registerRequest.isRegular*/ false
                
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            StudentDetail studentDetails = new StudentDetail()
            {
                Address = registerRequest.Address,
                PhoneNumber = registerRequest.Phone,
                StudentId = student.StudentId,
                AdditionalInformation = registerRequest.AdditionalInformation
            };
            _context.StudentDetails.Add(studentDetails);
            await _context.SaveChangesAsync(); 
            return Ok();
        }

        [HttpPost("SendActivationEmail")]
        public async Task<IActionResult> SendActivationEmail(string email)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            var emailSent = await SendActivationEmailAsync(email, account?.ActiveCode.Trim());
            if (!emailSent)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error sending email.");
            }

            return Ok("Activation email sent.");
        }
        [HttpPost("VerifyAccount")]
        public IActionResult VerifyAccount(string email, string activationCode)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email && x.ActiveCode.Trim() == activationCode.Trim());
            if (account == null)
            {
                return BadRequest("Invalid activation code or email.");
            }
            //var emailsent = SendActivationEmailAsync(email, activationCode);
            //if (!emailsent.Result)
            //{
            //    return StatusCode(500, "Error sending email.");
            //}

            account.IsActive = true;
            account.ActiveCode = null;

            _context.SaveChanges();

            return Ok("Account activated successfully.");
        }
        [HttpPost("FogotPassword")]
        public async Task<IActionResult> FogotPassword(string email)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }
            var newPassword= GenerateRandomPassword(8);
            var byteData = Encoding.UTF8.GetBytes(newPassword);
            var encryptedPassword = Convert.ToBase64String(byteData);
            account.Password = encryptedPassword;
             _context.SaveChangesAsync();
            var plainTextContent = $"Your new password is: {newPassword}";
            var htmlContent = $@"
        <h2>Reset your password</h2>
        <p>Your new password is: <strong>{newPassword}</strong></p>";
            var emailSent = await SendEmailAsync(email, "Reset your password", plainTextContent, htmlContent);
            return Ok("Activation email sent.");
        }
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword(string email, string newPassword)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            var byteData = Encoding.UTF8.GetBytes(newPassword);
            var encryptData = Convert.ToBase64String(byteData);
            account.Password = encryptData; 
            _context.SaveChanges();

            return Ok("Password changed successfully.");
        }
        [HttpGet("GetUserByUserId")]
        public IActionResult GetUserByUserId(int userId)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.AccountId == userId);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }

            return Ok(account);
        }
        private async Task<bool> SendActivationEmailAsync(string email, string activationCode)
        {
           
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Project", "toanbvhe163899@fpt.edu.vn"));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = "Activate your account";

            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Your activation code is: {activationCode}", // Plain text version
                HtmlBody = $@"
            <h2>Activate your account</h2>
            <p>Your activation code is: <strong>{activationCode}</strong></p>"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();
            using (var client = new  MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("toanbvhe163899@fpt.edu.vn", "iiyg fzdm ltsi pxsy");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                return true; 
        }
        private async Task<bool> SendEmailAsync(string email, string subject, string plainTextContent, string htmlContent)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Project", "toanbvhe163899@fpt.edu.vn"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;     

            var bodyBuilder = new BodyBuilder
            {
                TextBody = plainTextContent, // Plain text version
                HtmlBody = htmlContent       // HTML version
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("toanbvhe163899@fpt.edu.vn", "iiyg fzdm ltsi pxsy");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            return true;
        }
        private string GenerateRandomPassword(int length )
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            var random = new Random();
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            return new string(chars);
        }
    }
}
