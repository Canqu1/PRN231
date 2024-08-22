using BackEnd.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public StudentSubjectsDTO StudentSubjects { get; set; }
        public async Task<IActionResult> OnGetAsync(int studentId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5138/api/Student/students/{studentId}/subjects");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                StudentSubjects = JsonSerializer.Deserialize<StudentSubjectsDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Page();
            }

            return NotFound();
        } 
    }
}
