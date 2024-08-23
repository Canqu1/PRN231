using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace FrontEnd.Pages.Students
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public CreateStudent Student { get; set; }
        public string Message { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string StudentAPIUrl = "http://localhost:5138/api/Student";
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

                string stringData = JsonSerializer.Serialize(Student);
                var contentData = new StringContent(stringData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(StudentAPIUrl, contentData);
                if (response.IsSuccessStatusCode)
                {
                    Message = "Student updated successfully";
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
