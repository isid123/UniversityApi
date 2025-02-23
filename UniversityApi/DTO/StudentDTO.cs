using UniversityApi.Data;

namespace UniversityApi.DTO
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
    }
}
