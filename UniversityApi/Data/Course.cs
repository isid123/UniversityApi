namespace UniversityApi.Data
{
    public class Course
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime StartCourse { get; set; }
        public bool isTriennal;
        public List<Subject>? Subjects { get; set; }
    }
}
