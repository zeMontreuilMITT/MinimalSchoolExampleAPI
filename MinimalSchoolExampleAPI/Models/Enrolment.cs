namespace MinimalSchoolExampleAPI.Models
{
    public class Enrolment
    {
        public int Id { get; set; }
        public int Grade { get; set; }


        public Student Student { get; set; }
        public int StudentId { get; set; }


        public Course Course { get; set; }
        public int CourseId { get; set; }

        public Enrolment() { }

        public Enrolment(int id, Student student, Course course)
        {
            Id = id;
            Student = student;
            StudentId = student.Id;

            Course = course;
            CourseId = course.Id;
        }
    }
}

/* SELECT * FROM Student, Enrolment
 * INNER JOIN Enrolment
 * ON Student.Id = Enrolment.StudentId
 * WHERE Student.Id = 5
 */

// Student student = Students.First(s => s.Id == 5);
// student.Enrolments 
