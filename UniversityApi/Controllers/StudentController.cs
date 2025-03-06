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
        [HttpPost("subscribe/{courseId}")]
        public IActionResult SubscribeToCourse(StudentDTO studentDTO, int courseId)
        {
            /*lo studente può iscriversi al corso solo se tutti i suoi crediti sono definiti*/

            // Voglio controllare che lo studente esista senno lo creo 
            var existingStudent = ctx.Students
                .SingleOrDefault(s => s.Name == studentDTO.Name 
                && s.Surname == studentDTO.Surname);

            if (existingStudent == null)
            {
                var newStudent = mapper.MapDtoToEntity(studentDTO);
                ctx.Students.Add(newStudent);
                ctx.SaveChanges();

                return Ok(mapper.MapEntityToDto(newStudent));
            }
            var student = existingStudent;

            var existingCourse = ctx.Courses.Find(courseId);
            if (existingCourse == null) return BadRequest();

            int totalCreditsDefined = ctx.Subjects.Where(sub => sub.CourseId == courseId).Sum(sub => sub.Credits);

            int requiredCredits = existingCourse.isTriennal ? 180 : 120;

            if (totalCreditsDefined < requiredCredits)
            {
                return BadRequest("The course does not have the required number of credits to allow student enrollment");
            }

            var existingSubscription = ctx.Subscriptions
                .SingleOrDefault(s => s.StudentId == existingStudent.Id && s.CourseId == existingCourse.Id);

            if(existingSubscription != null)
            {
                return BadRequest("Student alredy joined this course.");
            }

            var subscription = new Subscription
            {
                StudentId = existingStudent.Id,
                CourseId = existingCourse.Id,
            };

            ctx.Subscriptions.Add(subscription);
            ctx.SaveChanges();

            return Ok("Student successfully joined the course.");
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
                ctx.Exams.RemoveRange(student.Exams); // rimuovo gli esami associati allo studente
            }

            ctx.Students.Remove(student);

            ctx.SaveChanges();
            return Ok("Student and their exams deleted!");

        }
    }
}
