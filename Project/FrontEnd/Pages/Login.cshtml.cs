using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using BackEnd.DTO;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Security.Claims;
using System.Xml.Linq;

namespace FrontEnd.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient client;
        private  string loginApiURl = "";
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public string Email { get; set; }
        public string uid { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public LoginModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            _tokenHandler = new JwtSecurityTokenHandler();

            loginApiURl = "http://localhost:5138/api";
        }
        [BindProperty]
        public LoginRequest LoginRequest { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }


        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {

            var login = new LoginRequest
            {
                Email = LoginRequest.Email,
                Password = LoginRequest.Password
            };
        
            var json = JsonSerializer.Serialize(login);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(loginApiURl + "/Auth/Login", data);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JWToken", token);
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(SanitizeToken(token));
                //HttpContext.Session.SetString("JWTokendecode", jwtToken.ToString());
                if (jwtToken != null)
                {
                    uid = jwtToken.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                    Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                    Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;


                    if (uid != null)
                    {
                        HttpContext.Session.SetString("UserId", uid);
                    }
                    if(Role!= null)
                    {
                        HttpContext.Session.SetString("Role", Role);
                    }
                    if (Name != null)
                    {
                        HttpContext.Session.SetString("Name", Name);
                    }
                    HttpResponseMessage responseAcc = await client.GetAsync($"{loginApiURl}/Auth/GetUserByUserId?userId={uid}");
                    if (responseAcc.IsSuccessStatusCode)
                    {
                        var userResponse = await responseAcc.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var user = JsonSerializer.Deserialize<Account>(userResponse, options);

                        if (user?.IsActive == false)
                        {
                            var activationEmailResponse = await client.PostAsync(
                            $"{loginApiURl}/Auth/SendActivationEmail?email={user.Email}",
                            null); 

                            if (activationEmailResponse.IsSuccessStatusCode)
                            {
                                return RedirectToPage("/VerifyAccount", new { email = user.Email });
                            }
                        }
                    }
               
                }

                if(Role == "admin")
                {
                    return RedirectToPage("/Admin/Subject");
                }
                else if (Role == "teacher")
                {
                    return RedirectToPage("Teacher/TeacherInfo/", new { userId = uid });
                }
                else if (Role == "student")
                {
                    return RedirectToPage("/Student/Index/", new {userId =uid});
                }
                else
                {
                    ErrorMessage = "Invalid login attempt";         
                    return Page();
                }
                //return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid login attempt";
                return Page();
            }
        }
        public string SanitizeToken(string token)
        {
            // Remove unwanted characters like escaped quotes
            return token.Replace("\\\"", "\"").Trim('\"');
        }


    }
}
