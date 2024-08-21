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
        public IActionResult Login(string email ,string password)
        {

            var byteData = Encoding.UTF8.GetBytes(password);
            var encryptData = Convert.ToBase64String(byteData);
            Account? account = _context.Accounts.FirstOrDefault(x => x.Email == email && x.Password == encryptData);
            if (account == null)
            {
                return Unauthorized();
            }
            var claims = new Claim[]
          {
              new Claim(ClaimTypes.Email, account.Email.ToString(), ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
              new Claim(ClaimTypes.Role, account.Type.ToString(), ClaimValueTypes.String, _configuration["Jwt:Issuer"]),
          };
            var key = new SymmetricSecurityKey(
             System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

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
                IsRegularStudent = registerRequest.isRegular
                
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

            var emailSent = await SendActivationEmailAsync(email, account.ActiveCode);
            if (!emailSent)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error sending email.");
            }

            return Ok("Activation email sent.");
        }
        [HttpPost("VerifyAccount")]
        public IActionResult VerifyAccount(string email, string activationCode)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email && x.ActiveCode == activationCode);
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
        public IActionResult FogotPassword(string email)
        {
            var account = _context.Accounts.FirstOrDefault(x => x.Email == email);
            if (account == null)
            {
                return BadRequest("Account not found.");
            }
            Guid guidActiveCode = Guid.NewGuid();
            account.ActiveCode = guidActiveCode.ToString().Trim();
            _context.SaveChanges();
            var emailSent = SendActivationEmailAsync(email, account.ActiveCode);
            if (!emailSent.Result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error sending email.");
            }
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
            account.Password = encryptData; // Implement your password hashing logic
            _context.SaveChanges();

            return Ok("Password changed successfully.");
        }
        private async Task<bool> SendActivationEmailAsync(string email, string activationCode)
        {
           
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Project", _configuration["Smtp:From"]));
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
                return true; // Email sent successfully
        }

    }
}
