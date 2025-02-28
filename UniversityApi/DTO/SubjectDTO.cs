namespace UniversityApi.DTO
{
    public class SubjectDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
    }
}
