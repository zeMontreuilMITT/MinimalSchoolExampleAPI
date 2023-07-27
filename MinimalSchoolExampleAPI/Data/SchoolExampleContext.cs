using Microsoft.EntityFrameworkCore;
using MinimalSchoolExampleAPI.Models;

namespace MinimalSchoolExampleAPI.Data
{
    public class SchoolExampleContext: DbContext
    {
        public SchoolExampleContext(DbContextOptions options) : base(options) { }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Enrolment> Enrolments { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;

    }
}
