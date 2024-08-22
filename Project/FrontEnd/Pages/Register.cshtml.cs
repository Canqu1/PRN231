using BackEnd.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace FrontEnd.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterRequest RegisterRequest { get; set; }
        private readonly HttpClient client;
        private readonly string _registerApiUrl = "";

        public RegisterModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            _registerApiUrl = "http://localhost:5138/api/Auth/Register";

        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (RegisterRequest.Password != RegisterRequest.RePassword)
            {
                ModelState.AddModelError("RegisterRequest.RePassword", "Passwords do not match.");
                return Page();
            }
            var json = JsonSerializer.Serialize(RegisterRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_registerApiUrl, data);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/VerifyAccount", new { email = RegisterRequest.Email });
            }
            else
            {
                // Handle failure case
                ModelState.AddModelError(string.Empty, "Registration failed.");
                return Page();
            }
        }
    }
}
