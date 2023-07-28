namespace MinimalSchoolExampleAPI.Models
{
    // Student Model
    /// <summary>
    /// Represent the Student Table of our database
    /// Each student Instance represents a single record of a student on that table
    /// </summary>
    public class Student
    {
        // primary key
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }

        // relational property
        public HashSet<Enrolment> Enrolments { get; set; } = new HashSet<Enrolment>();
     
        public Student()
        {

        }

        public Student(int studentNumber, string firstName, string lastName)
        {
            Id = studentNumber;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}