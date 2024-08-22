using BackEnd.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Students
{
    public class StudentInforModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StudentInforModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public StudentProfileDTO Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int studentId)
        {
            var client = _httpClientFactory.CreateClient();
            var studentResponse = await client.GetAsync($"http://localhost:5138/api/Student/students/{studentId}/profile");
            if (studentResponse.IsSuccessStatusCode)
            {
                var studentContent = await studentResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Đảm bảo không phân biệt chữ hoa, chữ thường
                };

                Student = JsonSerializer.Deserialize<StudentProfileDTO>(studentContent, options);

                return Page();
            }

            return NotFound();
        }
    }
}
