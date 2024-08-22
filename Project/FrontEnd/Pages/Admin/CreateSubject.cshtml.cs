using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FrontEnd.Pages.Admin
{
    [Authorize(Roles = "admin")]
    public class CreateSubjectModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public string SubjectName { get; set; }

        public CreateSubjectModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void OnGet()
        {
            // No need to initialize here
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(SubjectName), "SubjectName");

            try
            {
                var response = await _httpClient.PostAsync("http://localhost:5138/api/Subject", formContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Admin/Subject"); // Redirect to the correct page
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error: {response.ReasonPhrase} - {responseContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Request error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
            }

            return Page();
        }
    }
}
