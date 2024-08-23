using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FrontEnd.Pages.Students
{
    public class DetailModel : PageModel
    {
        [BindProperty]
        public Student UpdateStudent { get; set; }
        public Student Student { get; set; }
        public bool? IsEdit { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string StudentAPIUrl = "http://localhost:5138/api/Student";
        public string Message { get; set; }
        public async Task<ActionResult> OnGet(int studentId, bool? isUpdate)
        {
            HttpResponseMessage response = await httpClient.GetAsync(StudentAPIUrl + "/Detail/" + studentId);
            string stringData = await response.Content.ReadAsStringAsync();
            IsEdit = isUpdate;
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Student = JsonSerializer.Deserialize<Student>(stringData, option);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            string stringData = JsonSerializer.Serialize(UpdateStudent);
            var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync(StudentAPIUrl + $"/students/{UpdateStudent.StudentId}/profile", contentData);
            if (response.IsSuccessStatusCode)
            {
                Message = "Student updated successfully";
            }
            else
            {
                Message = "Error while calling Web API";
            }
            HttpResponseMessage response2 = await httpClient.GetAsync(StudentAPIUrl + "/detail/" + UpdateStudent.StudentId);
            string stringData2 = await response2.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Student = JsonSerializer.Deserialize<Student>(stringData, option);
            IsEdit = true;
            return Page();
        }
    }
}
