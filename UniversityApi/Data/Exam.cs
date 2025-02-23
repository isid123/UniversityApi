using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApi.Data
{
    [PrimaryKey(nameof(StudentId), nameof(SubjectId))]
    public class Exam
    {
        public int Grade { get; set; }

        public int StudentId { get; set; }
        public int SubjectId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student? student { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public Subject? subject { get; set; }

    }
}
