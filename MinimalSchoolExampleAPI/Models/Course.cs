namespace MinimalSchoolExampleAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseTitle { get; set; }
        
        private int _year;
        public int Year
        {
            get { return _year; }
            set
            {
                if(value < 1990 || value > 2050)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                else
                {
                    _year = value;
                }
            }
        }

        private int _passingGrade;

        public int PassingGrade { get { return _passingGrade; }
            set {
                if (value > 100 || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _passingGrade = value;
            } 
        }

        public HashSet<Enrolment> Enrolments = new HashSet<Enrolment>();

        public Course()
        {

        }

        public Course(int courseNumber, string courseTitle, int year, int passingGrade = 50)
        {
            Id = courseNumber;
            CourseTitle = courseTitle;
            Year = year;
            PassingGrade = passingGrade;
        }
    }
}
