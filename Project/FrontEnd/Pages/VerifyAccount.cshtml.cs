using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace FrontEnd.Pages
{
    public class VerifyAccountModel : PageModel

    {
        private readonly HttpClient client;
        private string loginApiURl = "";

        public VerifyAccountModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            loginApiURl = "http://localhost:5138/api";
        }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string ActivationCode { get; set; }
        public string Message { get; set; }
        public void OnGet()
        {
            Email = Request.Query["email"];
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var response = await client.PostAsync($"{loginApiURl}/Auth/VerifyAccount?email={Email}&activationCode={ActivationCode}", null);

            if (response.IsSuccessStatusCode)
            {
                Message = "Account successfully activated.";
                return RedirectToPage("/Index");
            }
            else
            {
                Message = "Failed to activate account. Please check your details and try again.";
                return Page();
            }

        }
    }
}
