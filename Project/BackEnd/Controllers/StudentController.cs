using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public StudentController(ProjectPRN231Context context)
        {
            _context = context;
        }

        [HttpGet("evaluation")]
        public async Task<IActionResult> GetStudentsWithEvaluations(int id)
        {
            var students = await _context.Students
                .Include(s => s.Evaluations)
                .Where(s => s.StudentId == id)
                .Select(s => new StudentRep
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Evaluation = s.Evaluations.Select(e => new EvaluationDTO
                    {
                        Grade = e.Grade,
                        AdditionalInformation = e.AdditionExplanation
                    }).ToList()
                })
                .ToListAsync();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentDetails(int id)
        {
            var student = await _context.Students.Include(s => s.StudentDetails)
                .Where(s => s.StudentId == id)
                .Select(s => new StudentDetailReqDTO
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Age = s.Age,
                    IsRegularStudent = s.IsRegularStudent,
                    Address = s.StudentDetails.FirstOrDefault().Address, // Lấy chi tiết đầu tiên
                    AdditionalInformation = s.StudentDetails.FirstOrDefault().AdditionalInformation,
                    Evaluation = s.Evaluations.Select(e => new EvaluationDTO
                    {
                        Grade = e.Grade,
                        AdditionalInformation = e.AdditionExplanation
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return Ok(student);
        }

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateStudentProfile(int id, [FromForm] StudentReq updateStudentProfileDTO)
        {
            var student = await _context.Students.Include(s => s.StudentDetails).FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            // Cập nhật thông tin student
            student.Name = updateStudentProfileDTO.Name;
            student.Age = updateStudentProfileDTO.Age;
            student.IsRegularStudent = updateStudentProfileDTO.IsRegularStudent;

            // Cập nhật thông tin chi tiết student
            if (student.StudentDetails != null)
            {
                student.StudentDetails.FirstOrDefault().Address = updateStudentProfileDTO.Address;
                student.StudentDetails.FirstOrDefault().AdditionalInformation = updateStudentProfileDTO.AdditionalInformation;
            }

            try
            {
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating profile" });
            }
        }

    }
}
