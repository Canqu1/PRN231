using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231_ProjectTest.Controllers
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



        //Get Student Profile Information
        [HttpGet("students/{studentId}/profile")]
        public async Task<IActionResult> GetStudentProfile(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.StudentDetails) // Include student details if needed
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound();
            }

            var studentProfile = new
            {
                student.StudentId,
                student.Name,
                student.Age,
                student.IsRegularStudent,
                student.AccountId,
                Details = student.StudentDetails.Select(sd => new
                {
                    sd.StudentDetailsId,
                    sd.Address,
                    sd.AdditionalInformation,
                    sd.PhoneNumber
                }).FirstOrDefault() // Assuming one student detail entry per student
            };

            return Ok(studentProfile);
        }





        //Update Student Profile

        [HttpPut("students/{studentId}/profile")]
        public async Task<IActionResult> UpdateStudentProfile(int studentId, [FromForm] StudentUpdateDto updatedStudent)
        {
            var student = await _context.Students
                .Include(s => s.StudentDetails) // Include student details to update them
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound();
            }

            // Update student properties
            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.IsRegularStudent = updatedStudent.IsRegularStudent;

            // Update student details if they exist
            var studentDetail = student.StudentDetails.FirstOrDefault();
            if (studentDetail != null)
            {
                studentDetail.Address = updatedStudent.Address;
                studentDetail.AdditionalInformation = updatedStudent.AdditionalInformation;
                studentDetail.PhoneNumber = updatedStudent.PhoneNumber;
            }
            else
            {
                // If no student detail exists, create a new one (if allowed by your logic)
                student.StudentDetails.Add(new StudentDetail
                {
                    Address = updatedStudent.Address,
                    AdditionalInformation = updatedStudent.AdditionalInformation,
                    PhoneNumber = updatedStudent.PhoneNumber,
                    StudentId = student.StudentId
                });
            }

            await _context.SaveChangesAsync();

            return Ok();
        }




        [HttpGet("students/{studentId}/evaluations")]
        public async Task<IActionResult> GetStudentEvaluations(int studentId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Teacher)
                .ToListAsync();

            if (evaluations == null)
            {
                return NotFound();
            }

            var evaluationDetails = evaluations.Select(e => new
            {
                e.EvaluationId,
                e.Grade,
                e.AdditionExplanation,
                TeacherName = e.Teacher != null ? e.Teacher.TeacherName : "No teacher assigned"
            });

            return Ok(evaluationDetails);
        }







        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }




    }
}
