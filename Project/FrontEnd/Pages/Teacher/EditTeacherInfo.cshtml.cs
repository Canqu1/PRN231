using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Teacher
{
    public class EditTeacherInfoModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditTeacherInfoModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TeacherDTO Teacher { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TeacherId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (TeacherId == 0)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"http://localhost:5138/api/Teacher/TeacherByID/{TeacherId}");
            if (!response.IsSuccessStatusCode)
            {
                // Log the response status code and reason
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return NotFound();
            }
            var teacherContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Đảm bảo không phân biệt chữ hoa, chữ thường
            };

            Teacher = JsonSerializer.Deserialize<TeacherDTO>(teacherContent, options);

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var jsonContent = JsonSerializer.Serialize(Teacher);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5138/api/Teacher/Teacher/{TeacherId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Teacher/TeacherInfo", new { userId = Teacher.AccountId });

            }

            return Page();
        }
    }
}

