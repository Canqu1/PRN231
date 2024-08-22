using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FrontEnd.Pages.Teacher
{
    public class EvaluationModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EvaluationModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<EvaluationDTO> Evaluations { get; set; }

        [BindProperty(SupportsGet = true)]
        public int TeacherId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (TeacherId == 0)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"http://localhost:5138/api/Teacher/Teacher/Evaluations/{TeacherId}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Đảm bảo không phân biệt chữ hoa, chữ thường
            };
            var jsonString = await response.Content.ReadAsStringAsync();
            Evaluations = JsonSerializer.Deserialize<List<EvaluationDTO>>(jsonString, options);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int evaluationId, string grade, string additionExplanation)
        {
            var updateDto = new UpdateEvaluationDTO
            {
                Grade = grade,
                AdditionExplanation = additionExplanation
            };

            var jsonContent = JsonSerializer.Serialize(updateDto);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"http://localhost:5138/api/Teacher/Evaluations/{evaluationId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage(new { TeacherId });
            }

            return Page();
        }
    }
}
