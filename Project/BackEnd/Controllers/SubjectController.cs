using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="subjectReq"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromForm] SubjectReqDTO subjectReq)
        {
            if (subjectReq == null || string.IsNullOrEmpty(subjectReq.SubjectName))
            {
                return BadRequest(new { message = "Invalid subject data." });
            }

            // Create a new Subject model object
            var newSubject = new Subject
            {
                SubjectName = subjectReq.SubjectName
            };

            try
            {
                // Add the new subject to the database
                _context.Subjects.Add(newSubject);
                await _context.SaveChangesAsync();

                // Return the created subject with a 201 status code
                return CreatedAtAction(nameof(GetAllSubjects), new { subjectId = newSubject.SubjectId }, newSubject);
            }
            catch (DbUpdateException ex)
            {
                // Handle any database update errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating subject", error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                return NotFound(new { message = "Subject not found." });
            }

            try
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                return NoContent(); // Return 204 No Content after successful deletion
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting subject", error = ex.Message });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{subjectId}")]
        public async Task<IActionResult> UpdateSubject(int subjectId, [FromForm] SubjectReqDTO subjectReq)
        {
            if (subjectReq == null || string.IsNullOrEmpty(subjectReq.SubjectName))
            {
                return BadRequest(new { message = "Invalid subject data." });
            }

            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                return NotFound(new { message = "Subject not found." });
            }

            subject.SubjectName = subjectReq.SubjectName;

            try
            {
                _context.Subjects.Update(subject);
                await _context.SaveChangesAsync();
                return Ok(subject); // Return the updated subject
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating subject", error = ex.Message });
            }
        }


    }
}
