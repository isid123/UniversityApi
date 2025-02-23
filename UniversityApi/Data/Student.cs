namespace UniversityApi.Data
{
    public class Student
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        List<Exam>? Exam { get; set; }

    }
}
