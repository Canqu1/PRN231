using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace FrontEnd.Pages
{
    public class ChangePasswordModel : PageModel
    {

        private readonly HttpClient client;
        private string apiUrl = "";
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string CurrentPassword { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public ChangePasswordModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            apiUrl = "http://localhost:5138/api";
        }

        public void OnGet( string email)
        {
            Email = email;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                Message = "New password and confirmation do not match.";
                return Page();
            }

            var response = await client.PostAsync($"{apiUrl}/Auth/ChangePassword?email={Email}&newPassword={NewPassword}",null);

            if (response.IsSuccessStatusCode)
            {
                Message = "Password changed successfully.";
            }
            else
            {
                Message = "Failed to change password.";
            }

            return Page();
        }
    }
}
