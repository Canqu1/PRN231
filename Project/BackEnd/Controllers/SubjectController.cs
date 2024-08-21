using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public SubjectController(ProjectPRN231Context context)
        {
            _context = context;
        }
        [HttpGet()]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subject = await _context.Subjects
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName
                })
                .ToListAsync();


            return Ok(subject);
        }

        //Get Student's Subjects
        [HttpGet("students/{studentId}/subjects")]
        public async Task<IActionResult> GetStudentSubjects(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.Subjects)  // Include Subjects for the student
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound();
            }

            var subjects = student.Subjects.Select(s => new
            {
                s.SubjectId,
                s.SubjectName
            });

            return Ok(subjects);
        }
    }
}
