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
            // Kiểm tra xem sinh viên có tồn tại hay không
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == studentId);

            if (!studentExists)
            {
                return NotFound();
            }

            // Lấy danh sách môn học của sinh viên đó
            var subjects = await _context.Subjects
                .Where(s => s.Students.Any(st => st.StudentId == studentId)) // Tìm các môn học liên kết với sinh viên
                .Select(s => new SubjectDTO
                {
                    SubjectId = s.SubjectId,
                    SubjectName = s.SubjectName
                })
                .ToListAsync();

            return Ok(subjects);
        }

    }
}
