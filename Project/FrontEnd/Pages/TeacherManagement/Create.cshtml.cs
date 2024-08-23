using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace FrontEnd.Pages.Teachers
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public CreateTeacher Teacher { get; set; }
        public string Message { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string TeacherAPIUrl = "http://localhost:5138/api/Teacher";
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return Page();
                }

                string stringData = JsonSerializer.Serialize(Teacher);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(TeacherAPIUrl + "/Create", contentData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Teacher updated successfully";
                }
                else
                {
                    Message = "Error while calling Web API";
                    return Page();
                }
                return RedirectToPage("./Index");

            }
            catch (Exception ex)
            {
                return Page();
            }
        }
    }
}
