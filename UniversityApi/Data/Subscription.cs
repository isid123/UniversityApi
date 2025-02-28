using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApi.Data
{
    [PrimaryKey(nameof(CourseId), nameof(StudentId))]
    public class Subscription
    {
        public int CourseId { get; set; }
        public int StudentId { get; set; }

        [ForeignKey(nameof(CourseId))]
        Course? Course { get; set; }
        [ForeignKey(nameof(StudentId))]
        Student? Student { get; set; }
    }
}
