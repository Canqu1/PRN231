using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;

namespace FrontEnd.Pages.Teachers
{
    public class IndexModel : PageModel
    {
        public List<Teacher> Teachers { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string StudentAPIUrl = "http://localhost:5138/api/Teacher";
        public async Task OnGet()
        {
            HttpResponseMessage response = await httpClient.GetAsync(StudentAPIUrl);
            string stringData = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Teachers = JsonSerializer.Deserialize<List<Teacher>>(stringData, option);
        }
    }
}
