using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace FrontEnd.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly HttpClient client;
        private string loginApiURl = "";

        public ForgotPasswordModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            loginApiURl = "http://localhost:5138/api";
        }

        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await client.PostAsync($"{loginApiURl}/Auth/FogotPassword?email={Email}", null);
            if (response.IsSuccessStatusCode)
            {
                Message = "Password reset email has been sent.";
                return RedirectToPage("/ChangePassword", new { email = Email });
            }
            else
            {
                Message = "Failed to send password reset email.";
            }
            return Page();
        }

      
    }
}
