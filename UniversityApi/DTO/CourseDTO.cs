namespace UniversityApi.DTO
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime StartCourse { get; set; }
        public bool isTriennal;
    }
}
