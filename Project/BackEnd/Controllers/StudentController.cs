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

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var students = await _context.Students
                .Include(x => x.StudentDetails)
                .Select(s => new
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Age = s.Age,
                    Address = s.StudentDetails.FirstOrDefault().Address, // Lấy chi tiết đầu tiên
                    PhoneNumber = s.StudentDetails.FirstOrDefault().PhoneNumber,
                    AdditionalInformation = s.StudentDetails.FirstOrDefault().AdditionalInformation,
                }).ToListAsync();

            return Ok(students);
        }

        [HttpGet("Detail/{studentId}")]
        public ActionResult GetStudent(int studentId)
        {
            var students =  _context.Students
                .Include(x => x.StudentDetails)
                .Select(s => new
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Age = s.Age,
                    Address = s.StudentDetails.FirstOrDefault().Address, // Lấy chi tiết đầu tiên
                    PhoneNumber = s.StudentDetails.FirstOrDefault().PhoneNumber,
                    AdditionalInformation = s.StudentDetails.FirstOrDefault().AdditionalInformation,
                }).FirstOrDefault(x => x.StudentId == studentId);

            return Ok(students);
        }
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
        [HttpGet("students/{studentId}/subjects")]
        public async Task<IActionResult> GetStudentSubjects(int studentId)
        {
            var studentData = await _context.Students
                .Where(s => s.StudentId == studentId)
                .Select(s => new
                {
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
            student.IsRegularStudent = updateStudentProfileDTO.IsRegularStudent;

            // Cập nhật thông tin chi tiết student
            var studentDetail = student.StudentDetails.FirstOrDefault();
            if (studentDetail != null)
            {
                studentDetail.Address = updateStudentProfileDTO.Address;
                studentDetail.AdditionalInformation = updateStudentProfileDTO.AdditionalInformation;
                studentDetail.PhoneNumber = updateStudentProfileDTO.PhoneNumber;
            }
            else
            {
                // If no student detail exists, create a new one (if allowed by your logic)
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

        [HttpPost]
        public IActionResult AddStudent([FromBody] CreateStudent studentReq)
        {
            try
            {
                var acc = _context.Accounts.FirstOrDefault(x => x.Email == studentReq.Email);
                if (acc != null)
                {
                    return BadRequest();
                }
                // Create a new account
                Guid guidActiveCode = Guid.NewGuid();
                var newAccount = new Account
                {
                    Email = studentReq.Email,
                    Password = studentReq.Password,
                    ActiveCode = guidActiveCode.ToString(),
                    IsActive = false,
                    Type = "student"
                };
                _context.Accounts.Add(newAccount);
                _context.SaveChanges();
                var st = new Student()
                {
                    AccountId = newAccount.AccountId,
                    Name = studentReq.Name,
                    Age = studentReq.Age,
                    IsRegularStudent = studentReq.IsRegularStudent,
                };
                _context.Students.Add(st);
                _context.SaveChanges();
                var detail = new StudentDetail()
                {
                    StudentId = st.StudentId,
                    Address = studentReq.Address,
                    AdditionalInformation = studentReq.AdditionalInformation,
                    PhoneNumber = studentReq.PhoneNumber
                };
                _context.StudentDetails.Add(detail);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
