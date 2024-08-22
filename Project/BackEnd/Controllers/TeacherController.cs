using BackEnd.DTO;
using BackEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PRN231_ProjectTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ProjectPRN231Context _context;

        public TeacherController(ProjectPRN231Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherDTO>>> GetTeachers()
        {
            return await _context.Teachers
                                 .Include(t => t.Evaluations)
                                 .Include(t => t.Account)
                                 .Select(t => new TeacherDTO
                                 {
                                     TeacherId = t.TeacherId,
                                     AccountId = t.AccountId,
                                     TeacherName = t.TeacherName,
                                     Email = t.Email,
                                     Department = t.Department,
                                     PhoneNumber = t.PhoneNumber,
                                     HireDate = t.HireDate
                                 })
                                 .ToListAsync();
        }

        [HttpGet("Teacher/{userId}")]
        public ActionResult<int> GetGvId(int userId)
        {
            var teacher = _context.Teachers.FirstOrDefault(s => s.AccountId == userId);
            if (teacher == null)
            {
                return NotFound("Teacher not exist");
            }

            return teacher.TeacherId;
        }
        [HttpGet("TeacherByID/{teacherId}")]
        public async Task<ActionResult<TeacherDTO>> GetTeacherById(int teacherId)
        {
            var teacher = await _context.Teachers
                                        .Include(t => t.Evaluations)
                                         .Include(t => t.Account)
                                        .FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null)
            {
                return NotFound("Teacher not exist");
            }

            var teacherDto = new TeacherDTO
            {
                TeacherId = teacher.TeacherId,
                AccountId = teacher.AccountId,
                TeacherName = teacher.TeacherName,
                Email = teacher.Email,
                Department = teacher.Department,
                PhoneNumber = teacher.PhoneNumber,
                HireDate = teacher.HireDate
            };

            return Ok(teacherDto);
        }


        [HttpPost]
        public async Task<ActionResult<TeacherDTO>> PostTeacher(TeacherDTO teacherDto)
        {
            var teacher = new Teacher
            {
                TeacherName = teacherDto.TeacherName,
                Email = teacherDto.Email,
                Department = teacherDto.Department,
                PhoneNumber = teacherDto.PhoneNumber
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTeachers), new { id = teacher.TeacherId }, teacherDto);
        }

        [HttpGet("{teacherId}/Student/{studentId}/Evaluations")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> GetEvaluationsForStudentByTeacher(int teacherId, int studentId)
        {
            var evaluations = await _context.Evaluations
                                            .Where(e => e.StudentId == studentId && e.TeacherId == teacherId)
                                            .Select(e => new EvaluationDTO
                                            {
                                                EvaluationId = e.EvaluationId,
                                                Grade = e.Grade,
                                                AdditionExplanation = e.AdditionExplanation,
                                                StudentName = e.Student.Name
                                            })
                                            .ToListAsync();

            if (!evaluations.Any())
            {
                return NotFound();
            }

            return evaluations;
        }

        [HttpPost("{teacherId}/Student/{studentId}/Evaluations")]
        public async Task<ActionResult<EvaluationDTO>> PostEvaluationForStudent(int teacherId, int studentId, CreateEvaluationDTO evaluationDto)
        {
            var evaluation = new Evaluation
            {
                TeacherId = teacherId,
                StudentId = studentId,
                Grade = evaluationDto.Grade,
                AdditionExplanation = evaluationDto.AdditionExplanation
            };

            _context.Evaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvaluationsForStudentByTeacher),
                                   new { teacherId = teacherId, studentId = studentId },
                                   new EvaluationDTO
                                   {
                                       EvaluationId = evaluation.EvaluationId,
                                       Grade = evaluation.Grade,
                                       AdditionExplanation = evaluation.AdditionExplanation
                                   });
        }

        [HttpPut("Evaluations/{evaluationId}")]
        public async Task<IActionResult> UpdateEvaluation(int evaluationId, [FromBody] UpdateEvaluationDTO updatedEvaluationDto)
        {
            var evaluation = await _context.Evaluations
                                           .Where(e => e.EvaluationId == evaluationId)
                                           .FirstOrDefaultAsync();

            if (evaluation == null)
            {
                return NotFound();
            }

            evaluation.Grade = updatedEvaluationDto.Grade;
            evaluation.AdditionExplanation = updatedEvaluationDto.AdditionExplanation;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationExists(evaluationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPut("Teacher/{teacherId}")]
        public async Task<IActionResult> UpdateTeacherInfo(int teacherId, [FromBody] UpdateTeacherDTO updateTeacherDto)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.TeacherId == teacherId);

            if (teacher == null)
            {
                return NotFound("Teacher not found");
            }

            teacher.TeacherName = updateTeacherDto.TeacherName;
            teacher.Email = updateTeacherDto.Email;
            teacher.Department = updateTeacherDto.Department;
            teacher.PhoneNumber = updateTeacherDto.PhoneNumber;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(teacherId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpGet("Teacher/Evaluations/{teacherId}")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> GetEvaluationsByTeacherId(int teacherId)
        {
            var evaluations = await _context.Evaluations
                                            .Where(e => e.TeacherId == teacherId)
                                            .Select(e => new EvaluationDTO
                                            {
                                                EvaluationId = e.EvaluationId,
                                                Grade = e.Grade,
                                                AdditionExplanation = e.AdditionExplanation,
                                                StudentName = e.Student.Name
                                            })
                                            .ToListAsync();

            if (!evaluations.Any())
            {
                return NotFound("No evaluations found for this teacher");
            }

            return Ok(evaluations);
        }


        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.TeacherId == id);
        }


        private bool EvaluationExists(int id)
        {
            return _context.Evaluations.Any(e => e.EvaluationId == id);
        }
    }
}
