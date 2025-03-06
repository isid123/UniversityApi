using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityApi.Data;
using UniversityApi.DTO;

namespace UniversityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly UniversityDbContext ctx;
        private readonly Mapper mapper;

        public CourseController(UniversityDbContext ctx, Mapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ctx.Courses.ToList().ConvertAll(mapper.MapEntityToDto));
        }
        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            var courses = ctx.Courses.Find(id);
            if (courses == null) return BadRequest();

            return Ok(mapper.MapEntityToDto(courses));
        }
        [HttpGet("most-student")]
        public IActionResult GetTopCCourses()
        {
            var topCourses = ctx.Subscriptions
                .GroupBy(s => s.CourseId)
                .Select(g => new
                {
                    CourseId = g.Key,
                    StudentCount = g.Count(),
                    CourseName = ctx.Courses
                    .Where(c => c.Id == g.Key)
                    .Select(c => c.Name)
                    .SingleOrDefault()
                }).OrderByDescending(c => c.StudentCount)
                .Take(3)
                .ToList();

            return Ok(topCourses);
        }

        /*POST SECTION*/
        [HttpPost("courses")]
        public IActionResult CreateCourse([FromBody] Course newCourse)
        {
            if (newCourse.StartCourse <= DateTime.Now)
                return BadRequest("The course start date must be in the future.");           

            ctx.Courses.Add(newCourse);
            ctx.SaveChanges();

            return Ok();
        }

        /*PUT SECTION*/
        [HttpPut("{id}")]
        public IActionResult UpdateCourse(int id, [FromBody] Course updatedCourse)
        {
            var existingCourse = ctx.Courses.Find(id);

            if (existingCourse == null)
                return NotFound("Course not found.");


            if (existingCourse.StartCourse <= DateTime.Now)
            {
                return BadRequest("The course has already started and cannot be modified.");
            }

            existingCourse.Name = updatedCourse.Name;
            existingCourse.StartCourse = updatedCourse.StartCourse;
            existingCourse.isTriennal = updatedCourse.isTriennal;

            ctx.SaveChanges();

            return Ok(existingCourse);
        }

        /*Delete Section*/
        [HttpDelete("{id}")]
        public IActionResult DeleteCourse(int id)
        {
            var courseToDelete = ctx.Courses.Find(id);

            if (courseToDelete == null)
                return NotFound();

            if (courseToDelete.StartCourse <= DateTime.Now)
            {
                return BadRequest("The course has already started and cannot be deleted.");
            }

            ctx.Courses.Remove(courseToDelete);
            ctx.SaveChanges();

            return Ok("Course successfully deleted.");
        }
    }
}
