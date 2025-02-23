using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.DTO;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly UniversityDbContext ctx;
        private readonly Mapper mapper;

        public StudentController(UniversityDbContext ctx, Mapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        /* GET SECTION */
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ctx.Students.ToList().ConvertAll(mapper.MapEntityToDto));
        }
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            var student = ctx.Students.Find(id);
            if (student == null) return BadRequest();

            return Ok(mapper.MapEntityToDto(student));
        }
        [HttpGet("top3")]
        public IActionResult GetTop()
        {
            var student = ctx.Students.Select(s => new
            {
                s.Id,
                s.Name,
                s.Surname,
                Average = (double)s.Exams.Sum(e => e.Grade * e.Subject.Credits) / s.Exams.Sum(e => e.Subject.Credits)
            }).OrderByDescending(s => s.Average)
            .Take(3);

            return Ok(student);
        }
        [HttpGet("worst-Subject")]
        public IActionResult GetWorst()
        {
            var subject = ctx.Subjects
                .Where(s => s.Exams.Any())
                .Select(s => new
            {
                s.Title,
                s.Credits,
                Average = (double)s.Exams.Sum(e => e.Grade * e.Subject.Credits) / s.Exams.Sum(e => e.Subject.Credits)
            }).OrderBy(s => s.Average)
            .Take(3);

            return Ok(subject);
        }
        [HttpGet("future-average/{id}")]
        public IActionResult GetFutureAverage(int id)
        {
            int MaxCredits = 180;

            var student = ctx.Students
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Surname,
                    CurrentCredits = s.Exams.Sum(e => e.Subject.Credits),
                    CurrentWeightedSum = s.Exams.Sum(e => e.Grade * e.Subject.Credits)
                })
                .SingleOrDefault();

            if (student == null)
            {
                return BadRequest("Studente non trovato.");
            }

            int remainingCredits = MaxCredits - student.CurrentCredits;

            if (remainingCredits <= 0)
            {
                return Ok(new { message = "Lo studente ha già raggiunto i 180 crediti!" });
            }

            double requiredWeightedSum = 28 * MaxCredits; 
            double futureAvg = (requiredWeightedSum - student.CurrentWeightedSum) / remainingCredits;

            return Ok(new
            {
                student.Id,
                student.Name,
                student.Surname,
                student.CurrentCredits,
                remainingCredits,
                RequiredFutureAverage = futureAvg > 30 ? 30 : futureAvg /*max is 30 credit for uni*/
            });
        }


        /* POST SECTION */


    }
}
