using FrontEnd.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Text.Json;

namespace FrontEnd.Pages.Students
{
    public class IndexModel : PageModel
    {
        public List<Student> Student { get; set; } = default!;
        [BindProperty(SupportsGet = true, Name = "ipp")]
        public int ItemsPerPage { get; set; } = 10;
        //public const int ITEMS_PER_PAGE = 10;
        [BindProperty(SupportsGet = true, Name = "p")]

        public int currentPage { get; set; }

        public int countPages { get; set; }
        
        [BindProperty] public int? ClassId { get; set; }
        private readonly HttpClient httpClient = new HttpClient();
        private string StudentAPIUrl = "http://localhost:5138/api/Student";
        public async Task<ActionResult> OnGet()
        {
            HttpResponseMessage response = await httpClient.GetAsync(StudentAPIUrl);
            string stringData = await response.Content.ReadAsStringAsync();

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Student = JsonSerializer.Deserialize<List<Student>>(stringData, option);
            return Page();
        }
    }
}
