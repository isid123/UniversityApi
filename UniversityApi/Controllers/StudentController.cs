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
        [HttpGet("worst3")]
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
        [HttpGet("FutureAvg/{id}")]
        public IActionResult GetFutureAvg(int id)
        {

            Student? student = ctx.Students
                .Include(s => s.Exams)
                .ThenInclude(e => e.Subject)
                .SingleOrDefault(s => s.Id == id);
            if (student == null)
                return BadRequest();
            int totCredits = ctx.Subjects.Sum(s => s.Credits);
            int actualCredits = student.Exams.Sum(e => e.Subject.Credits);
            int weightedGrades = student.Exams.Sum(e => e.Grade * e.Subject.Credits);

            double result = Math.Max((28 * totCredits - weightedGrades) / (totCredits - actualCredits), 18);
            if (result > 30)
                return Ok("impossible");
            return Ok(result);
        }



        /* POST SECTION */
        [HttpPost("get-many")]
        /* Permette di ottenere una lista di utenti specifici in base agli ID*/
        public IActionResult GetMany([FromBody] List<int> ids)
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
        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody] StudentDTO studentDTO)
        {
            var student = ctx.Students.Find(id);
            if (student == null) return BadRequest($"No student with id: {id} founded");

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

            if (student.Exams.Any())
            {
                ctx.exams.RemoveRange(student.Exams); // rimuovo gli esami associati allo studente
            }

            ctx.Students.Remove(student);

            ctx.SaveChanges();
            return Ok("Student and their exams deleted!");

        }
    }
}
