using BackEnd.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace FrontEnd.Controllers
{
    public class StudentsController : Controller
    {
        private readonly HttpClient client;
        public readonly string ApiBaseUrl = "http://localhost:5000/api/Student";
        public StudentsController()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET: Students
        public async Task<IActionResult> Index(int id)
        {
            var response = await client.GetStringAsync(ApiBaseUrl);
            var students = System.Text.Json.JsonSerializer.Deserialize<List<StudentRep>>(response);

            return View(students);
        }
    }
}
