using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace FrontEnd.Pages.Teachers
{
    public class DetailModel : PageModel
    {
        [BindProperty]
        public Teacher Teacher { get; set; }
        public bool? IsEdit { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string TeacherAPIUrl = "http://localhost:5138/api/Teacher";
        public string Message { get; set; }
        public async Task OnGet(int userId, bool? isUpdate)
        {
            HttpResponseMessage response = await httpClient.GetAsync(TeacherAPIUrl + "/ById/" + userId);
            string stringData = await response.Content.ReadAsStringAsync();
            IsEdit = isUpdate;
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Teacher = JsonSerializer.Deserialize<Teacher>(stringData, option);
        }

        public async Task OnPost()
        {
            if(!ModelState.IsValid)
            {
                return;
            }
            string stringData = JsonSerializer.Serialize(Teacher);
            var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
            Console.WriteLine(stringData);
            HttpResponseMessage response = await httpClient.PutAsync(TeacherAPIUrl, contentData);
            if (response.IsSuccessStatusCode)
            {
                Message = "Student updated successfully";
            }
            else
            {
                Message = "Error while calling Web API";
            }
            HttpResponseMessage response2 = await httpClient.GetAsync(TeacherAPIUrl + "/ById/" + Teacher.TeacherId);
            string stringData2 = await response2.Content.ReadAsStringAsync();
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Teacher = JsonSerializer.Deserialize<Teacher>(stringData, option);
            IsEdit = true;
        }
    }
}
