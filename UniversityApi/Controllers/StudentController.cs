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
            if (student == null) return BadRequest($"No student with id: {id} founded!");

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

            if(student == null) return BadRequest("No student founded");

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
                return BadRequest("Student not founded.");
            }

            int remainingCredits = MaxCredits - student.CurrentCredits;

            if (remainingCredits <= 0)
            {
                return Ok(new { message = "Student already reached 180 credits!" });
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
        [HttpPost("get-many")]

        /* Permette di ottenere una lista di utenti specifici in base agli ID*/
        public IActionResult GetMany(List<int> ids)
        {
            var resultLinq = (from s in ctx.Students // tutti gli student dal db
                              join id in ids         // join con la lista ID solo 
                              on s.Id equals id      // se la condizione sotto viene rispettata
                              select new
                              {
                                  s.Id,
                                  s.Name,
                                  s.Surname,
                                  Exams = s.Exams.Select(e => new             // la select mi serve per ritornare le informazioni che voglio (senno sarebbero null)
                                  {
                                      SubjectTitle = e.Subject.Title,
                                      e.Grade

                                  }).ToList()
                              }).ToList();
            return Ok(resultLinq);
        }

        /*PUT SECTION*/
        [HttpPut]
        public IActionResult Update(StudentDTO studentDTO)
        {
            var student = ctx.Students.Find(studentDTO.Id);
            if (studentDTO == null) return BadRequest();

            student.Name = studentDTO.Name;
            student.Surname = studentDTO.Surname;
            ctx.SaveChanges();
            return Ok();
        }

        /*DELETE SECTION*/

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var student = ctx.Students
                .Include(s => s.Exams) // carico gli esami dello studente
                .SingleOrDefault(s => s.Id == id);

            if(student == null) return BadRequest("Student not founded");

            ctx.exams.RemoveRange(student.Exams); // rimuovo gli esami associati allo studente

            ctx.Students.Remove(student);

            ctx.SaveChanges();
            return Ok("Student and their exams deleted!");

        }
    }
}
