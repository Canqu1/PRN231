using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Teacher
{
    public class TeacherInfoModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeacherInfoModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public TeacherDTO Teacher { get; set; }

        public async Task<IActionResult> OnGetAsync(int userId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5138/api/Teacher/Teacher/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var teacherId = JsonSerializer.Deserialize<int>(responseContent);

                var teacherResponse = await client.GetAsync($"http://localhost:5138/api/Teacher/TeacherByID/{teacherId}");
                if (teacherResponse.IsSuccessStatusCode)
                {
                    var teacherContent = await teacherResponse.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Đảm bảo không phân biệt chữ hoa, chữ thường
                    };

                    Teacher = JsonSerializer.Deserialize<TeacherDTO>(teacherContent, options);

                    return Page();
                }
            }
            return NotFound();
        }

    }
}