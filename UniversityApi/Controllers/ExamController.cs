using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.DTO;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly UniversityDbContext ctx;
        private readonly Mapper mapper;

        public ExamController(UniversityDbContext ctx, Mapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        /*GET SECTION*/
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ctx.Exams.ToList().ConvertAll(mapper.MapEntityToDto));
        }
        [HttpGet("{studentId}/{subjectId}")] 
        public IActionResult GetSingle(int studentId, int subjectId) 
        {
            Exam? result = ctx.Exams.Find(studentId, subjectId);
            if (result == null) return BadRequest($"Exam with studentId: {studentId} and subjectId: {subjectId} not founded");

            return Ok(mapper.MapEntityToDto(result));
        }
        [HttpGet("top3")]
        public IActionResult GetTop()
        {
            var exams = ctx.Exams.Select(e => new
            {
                Title = e.Subject.Title,
                e.SubjectId,
                e.StudentId,
                e.Grade,
            }).OrderByDescending(e => e.Grade).Take(3);

            return Ok(exams);
        }

        [HttpGet("worst3")]
        public IActionResult GetWorst()
        {
            var exams = ctx.Exams.Select(e => new
            {
                Title = e.Subject.Title,
                e.SubjectId,
                e.StudentId,
                e.Grade,
            }).OrderBy(e => e.Grade).Take(3);

            return Ok(exams);
        }

        /*POST SECTION*/
        [HttpPost]
        public IActionResult Create([FromBody] ExamDTO examDTO)
        {
            var entity = mapper.MapDtoToEntity(examDTO);

            ctx.Exams.Add(entity);
            ctx.SaveChanges();
            return Created("", mapper.MapEntityToDto(entity));
        }
        [HttpPost("get-many")]
        public IActionResult GetMany([FromBody] List<int> ids)
        {
            var result = (from e in ctx.Exams
                          join subjId in ids 
                          on e.SubjectId equals subjId
                          group e by new { e.SubjectId, e.Subject.Title } into g
                          select new
                          {
                              g.Key.SubjectId,
                              Title = g.Key.Title,
                              StudentCount = g.Count(),
                              AverageGrade = (double)g.Average(e => e.Grade)
                          }).ToList();

            return Ok(result);

        }
        /*PUT SECTION*/
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ExamDTO examDTO)
        {
            var entity = ctx.Exams.Find(id);
            if (entity == null) return BadRequest($"Exam with id: {id} not founded");

            entity.Grade = examDTO.Grade;

            ctx.SaveChanges();
            return Ok();
        }
        /*DELETE SECTION*/
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = ctx.Exams.Find(id);
            if (entity == null) return BadRequest($"Exam with id: {id} not founded");

            ctx.Exams.Remove(entity);
            ctx.SaveChanges();
            return Ok("Exam Deleted!");
        }
    }
}
