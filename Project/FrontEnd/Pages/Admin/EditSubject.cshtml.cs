using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
 // Replace with your actual namespace

namespace FrontEnd.Pages.Admin
{
    public class EditSubjectModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditSubjectModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public SubjectReqDTO Subject { get; set; }

        [FromRoute]
        public int SubjectId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (SubjectId == 0)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"http://localhost:5138/api/Subject/{SubjectId}");
            if (!response.IsSuccessStatusCode)
            {
                // Log the response status code and reason
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return NotFound();
            }

            var subjectContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignore case sensitivity
            };

            Subject = JsonSerializer.Deserialize<SubjectReqDTO>(subjectContent, options);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var jsonContent = JsonSerializer.Serialize(Subject);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5138/api/Subject/{SubjectId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Subject/SubjectInfo", new { subjectId = SubjectId });
            }

            return Page();
        }
    }
}
