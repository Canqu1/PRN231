using BackEnd.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontEnd.Pages.Students
{
    public class EditProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditProfileModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string StudentId { get; set; }
        [BindProperty]
        public StudentProfileDTO Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int studentId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://localhost:5138/api/Student/students/{studentId}/profile");

            if (response.IsSuccessStatusCode)
            {
                var studentContent = await response.Content.ReadAsStringAsync();
                Student = JsonSerializer.Deserialize<StudentProfileDTO>(studentContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _httpClientFactory.CreateClient();

            // Chuyển đổi StudentId thành int nếu cần
            if (!int.TryParse(StudentId, out var studentId))
            {
                ModelState.AddModelError("", "Invalid Student ID");
                return Page();
            }

            var updateStudent = new StudentReq
            {
                Name = Student.Name,
                AccountID = Student.Userid,
                Age = Student.Age,
                Address = Student.Details?.Address,
                PhoneNumber = Student.Details?.PhoneNumber,
                AdditionalInformation = Student.Details?.AdditionalInformation
            };

            var response = await client.PutAsJsonAsync($"http://localhost:5138/api/Student/students/{studentId}/profile", updateStudent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToPage("/Students/StudentInfor", new { studentId = studentId });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error updating student profile: {errorContent}");
            return Page();
        }

    }
}
