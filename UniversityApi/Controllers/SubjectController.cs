using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.DTO;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly UniversityDbContext ctx;
        private readonly Mapper mapper;
        public SubjectController(UniversityDbContext ctx, Mapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        /*GET SECTION*/
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ctx.Subjects.ToList().ConvertAll(mapper.MapEntityToDto));
        }
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            var subject = ctx.Subjects.Find(id);
            if (subject == null) return BadRequest($"No subject with id: {id} founded!");
            return Ok(mapper.MapEntityToDto(subject));
        }
        [HttpGet("worst3")]
        public IActionResult GetWorstSubject()
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
        /*POST SECTION*/
        [HttpPost]
        public IActionResult Create([FromBody] SubjectDTO subjectDTO)
        {
            var entity = mapper.MapDtoToEntity(subjectDTO);

            ctx.Subjects.Add(entity);
            ctx.SaveChanges();
            return Created("", mapper.MapEntityToDto(entity));
        }
        [HttpPost("get-many")]
        public IActionResult GetMany([FromBody] List<int> ids)
        {
            var result = (from sub in ctx.Subjects
                          join id in ids
                          on sub.Id equals id
                          select new
                          {
                              sub.Id,
                              sub.Title,
                              sub.Credits,

                          }).ToList();
            return Ok(result);
        }
        /*PUT SECTION*/
        [HttpPut("{id}")]
        public IActionResult Update(int id,[FromBody] SubjectDTO subjectDTO)
        {
            var entity = ctx.Subjects.Find(id);
            if (entity == null) return BadRequest($"No subject with id: {subjectDTO.Id} founded!");

            entity.Title = subjectDTO.Title;
            entity.Credits = subjectDTO.Credits;

            ctx.SaveChanges();
            return Ok();
        }
        /*DELETE SECTION*/
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = ctx.Subjects.Include(s => s.Exams).SingleOrDefault(s => s.Id == id);

            if (entity == null) return BadRequest("Subject not founded");

            if (entity.Exams.Any())
            {
                ctx.exams.RemoveRange(entity.Exams);
            }

            ctx.Subjects.Remove(entity);
            ctx.SaveChanges();
            return Ok();
        }
    }
}
