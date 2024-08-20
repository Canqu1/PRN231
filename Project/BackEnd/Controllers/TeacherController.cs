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
                                 .Select(t => new TeacherDTO
                                 {
                                     TeacherId = t.TeacherId,
                                     TeacherName = t.TeacherName,
                                     Email = t.Email,
                                     Department = t.Department,
                                     PhoneNumber = t.PhoneNumber
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
                                                AdditionExplanation = e.AdditionExplanation
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

        [HttpPut("{teacherId}/Student/{studentId}/Evaluations/{evaluationId}")]
        public async Task<IActionResult> UpdateEvaluation(int teacherId, int studentId, int evaluationId, [FromBody] UpdateEvaluationDTO updatedEvaluationDto)
        {
            var evaluation = await _context.Evaluations
                                           .Where(e => e.TeacherId == teacherId && e.StudentId == studentId && e.EvaluationId == evaluationId)
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

        private bool EvaluationExists(int id)
        {
            return _context.Evaluations.Any(e => e.EvaluationId == id);
        }
    }
}
