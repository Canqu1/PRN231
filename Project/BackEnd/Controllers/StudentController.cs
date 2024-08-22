using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("Student/{userID}")]
        public ActionResult<int> GetStudentId(int userID)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentId == userID);
            if (student == null)
            {
                return NotFound("Student not exist");
            }

            return student.StudentId;
        }
        // [Authorize(Roles = ("student"))]
        [HttpGet("evaluation")]
        public async Task<IActionResult> GetStudentsWithEvaluations(int studentId)
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
        // [Authorize(Roles = ("student"))]
        [HttpGet("students/{studentId}/subjects")]
        public async Task<IActionResult> GetStudentSubjects(int studentId)
        {
            var studentData = await _context.Students
                .Where(s => s.AccountId == studentId)
                .Select(s => new
                {
                    id = s.StudentId,
                    StudentName = s.Name,
                    Subjects = s.Subjects.Select(sub => new
                    {
                        SubjectName = sub.SubjectName,
                        Evaluations = sub.Students
                            .Where(stu => stu.StudentId == studentId)
                            .SelectMany(stu => stu.Evaluations
                                .Where(e => e.TeacherId != null) // Đảm bảo có giáo viên
                                .Select(e => new
                                {
                                    e.Grade,
                                    e.AdditionExplanation,
                                    TeacherName = e.Teacher != null ? e.Teacher.TeacherName : "No teacher assigned"
                                })
                            ).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (studentData == null || !studentData.Subjects.Any())
            {
                return NotFound(new { message = "Student or subjects not found" });
            }

            // Tạo đầu ra theo định dạng mong muốn
            var result = new
            {
                StudentID = studentData.id,
                StudentName = studentData.StudentName,
                Subjects = studentData.Subjects.SelectMany(sub => sub.Evaluations.Select(e => new
                {
                    SubjectName = sub.SubjectName,
                    e.Grade,
                    e.AdditionExplanation,
                    e.TeacherName
                }))
            };

            return Ok(result);
        }

        // [Authorize(Roles = ("student"))]
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentDetails(int studentId)
        {
            var student = await _context.Students.Include(s => s.StudentDetails)
                .Where(s => s.StudentId == studentId)
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
                        AdditionExplanation = e.AdditionExplanation
                    }).ToList()
                })
                .FirstOrDefaultAsync();
            return Ok(student);
        }
        // [Authorize(Roles = ("student"))]
        //Get Student Profile Information
        [HttpGet("students/{studentId}/profile")]
        public async Task<IActionResult> GetStudentProfile(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.StudentDetails) // Bao gồm chi tiết sinh viên nếu cần
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
        //[Authorize(Roles = ("student"))]
        [HttpPut("students/{studentId}/profile")]
        public async Task<IActionResult> UpdateStudentProfile(int studentId, [FromBody] StudentReq updateStudentProfileDTO)
        {
            var student = await _context.Students.Include(s => s.StudentDetails).FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            // Cập nhật thông tin student
            student.Name = updateStudentProfileDTO.Name;
            student.Age = updateStudentProfileDTO.Age;

            // Cập nhật hoặc tạo mới thông tin chi tiết student
            var studentDetail = student.StudentDetails.FirstOrDefault();
            if (studentDetail != null)
            {
                studentDetail.Address = updateStudentProfileDTO.Address;
                studentDetail.AdditionalInformation = updateStudentProfileDTO.AdditionalInformation;
                studentDetail.PhoneNumber = updateStudentProfileDTO.PhoneNumber;
            }
            else
            {
                // Nếu không tồn tại StudentDetail thì tạo mới
                student.StudentDetails.Add(new StudentDetail
                {
                    Address = updateStudentProfileDTO.Address,
                    AdditionalInformation = updateStudentProfileDTO.AdditionalInformation,
                    PhoneNumber = updateStudentProfileDTO.PhoneNumber,
                    StudentId = student.StudentId
                });
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
