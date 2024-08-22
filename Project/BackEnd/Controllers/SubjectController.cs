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

        [HttpGet("subjects/{subjectId}/students")]
        public async Task<IActionResult> GetStudentsBySubject(int subjectId)
        {
            // Truy vấn lấy thông tin về môn học, danh sách sinh viên và đánh giá
            var subjectData = await _context.Subjects
                .Where(sub => sub.SubjectId == subjectId)
                .Select(sub => new
                {
                    SubjectName = sub.SubjectName,
                    Students = sub.Students.Select(stu => stu.Evaluations
                        .Where(e => e.TeacherId != null) // Đảm bảo có giáo viên
                        .Select(e => new
                        {
                            StudentName = stu.Name,
                            e.Grade,
                            e.AdditionExplanation,
                            TeacherName = e.Teacher != null ? e.Teacher.TeacherName : "No teacher assigned"
                        })
                    ).SelectMany(x => x).ToList() // Kết hợp tất cả đánh giá của sinh viên
                })
                .FirstOrDefaultAsync();

            if (subjectData == null || !subjectData.Students.Any())
            {
                return NotFound(new { message = "Subject or students not found" });
            }

            // Tạo đầu ra theo định dạng mong muốn
            var result = new
            {
                SubjectName = subjectData.SubjectName,
                Students = subjectData.Students
            };

            return Ok(result);
        }

    }
}
