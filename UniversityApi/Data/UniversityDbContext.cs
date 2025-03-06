using Microsoft.EntityFrameworkCore;

namespace UniversityApi.Data
{
    public class UniversityDbContext : DbContext
    {
        /* constructors */
        public UniversityDbContext() : base() { }
       
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }


        /* tables */
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
