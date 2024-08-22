using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontEnd.Pages.Admin
{
    public class SubjectListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public SubjectListModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<SubjectReqDTO> Subjects { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync("http://localhost:5138/api/Subject");

            if (response.IsSuccessStatusCode)
            {
                var subjectContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                Subjects = JsonSerializer.Deserialize<List<SubjectReqDTO>>(subjectContent, options);
                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int subjectId, string subjectName)
        {
            var updateDto = new UpdateSubjectDTO
            {
                SubjectName = subjectName
            };

            var jsonContent = JsonSerializer.Serialize(updateDto);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5138/api/Subject/{subjectId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }

            return Page();
        }
    }
}
