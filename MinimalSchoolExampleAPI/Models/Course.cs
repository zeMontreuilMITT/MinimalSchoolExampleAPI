namespace MinimalSchoolExampleAPI.Models
{
    public class Course
    {
        public int CourseNumber { get; set; }
        public string CourseTitle { get; set; }
        public HashSet<Enrolment> Enrolments = new HashSet<Enrolment>();

        public Course()
        {

        }

        public Course(int courseNumber, string courseTitle)
        {
            CourseNumber = courseNumber;
            CourseTitle = courseTitle;
        }
    }
}
