namespace MinimalSchoolExampleAPI.Models
{
    public class Enrolment
    {
        public int Id { get; set; }
        public int Grade { get; set; }


        public Student Student { get; set; }
        public int StudentNumber { get; set; }


        public Course Course { get; set; }
        public int CourseNumber { get; set; }

        public Enrolment() { }

        public Enrolment(int id, Student student, Course course)
        {
            Id = id;
            Student = student;
            StudentNumber = student.StudentNumber;

            Course = course;
            CourseNumber = course.CourseNumber;
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
