using Microsoft.AspNetCore.Http.Json;
using MinimalSchoolExampleAPI.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// STUDENTS
app.MapGet("/students", () =>
{
    return Results.Ok(InternalDatabase.Students);
});

app.MapGet("/students/{number}", (int number) =>
{
    try
    {
        // find student in database
        Student student = InternalDatabase.Students.First(s => s.StudentNumber == number);
        return Results.Ok(student);

    } catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    } catch(Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/students/grades/add", (int courseId, int studentId, int newGrade) =>
{
    try
    {
       // find the correct enrolment
        Enrolment foundEnrolment = InternalDatabase.Enrolments.First(e =>
        {
            return e.CourseNumber == courseId && e.StudentNumber == studentId;
        });

        if(newGrade <= 100 && newGrade >= 0)
        {
            foundEnrolment.Grade = newGrade;
            // return Results.Accepted
            return Results.Ok(foundEnrolment);
        } else
        {
            throw new ArgumentOutOfRangeException(nameof(newGrade));
        }
    } 
    catch(InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(ex.Message);
    } 
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }

});


// COURSES
app.MapGet("courses/averagestudentcount", () => {
    return InternalDatabase.Courses.Average(c => c.Enrolments.Count);
});


app.Run();


static class InternalDatabase
{
    private static int _pkCount = 1;

    // COLLECTIONS
    public static HashSet<Student> Students { get; set; } = new HashSet<Student>();
    public static HashSet<Course> Courses { get; set; } = new HashSet<Course>();
    public static HashSet<Enrolment> Enrolments { get; set; } = new HashSet<Enrolment>();

    static InternalDatabase()
    {
        _seedMethodStudents();
        _seedCourseMethod();
        _seedEnrolmentMethod();
    }

    // METHODS
    public static void CreateStudent(string firstName, string lastName)
    {
        if(String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
        {
            throw new ArgumentOutOfRangeException("Student name cannot be empty.");
        }

        Student newStudent = new Student(_pkCount++, firstName, lastName);
        Students.Add(newStudent);
    }

    private static void _seedMethodStudents()
    {
        CreateStudent("Example", "Student");
        CreateStudent("Seed Method", "Person");
        CreateStudent("Daffy", "Duck");
        CreateStudent("Elmer", "Fudd");
    }

    public static void CreateCourse(string title)
    {
        if (!String.IsNullOrEmpty(title))
        {
            Course course = new Course(_pkCount++, title);
            Courses.Add(course);
        }
        else
        {
            throw new ArgumentOutOfRangeException("Title must have a value.");
        }
    }
    private static void _seedCourseMethod()
    {
        CreateCourse("Intro to Philosophy");
        CreateCourse("Advanced Utilitarianism");
        CreateCourse("Descartes for Dummies");
    }

    public static void CreateEnrolment(Student student, Course course)
    {
        Enrolment newEnrolment = new Enrolment(_pkCount++, student, course);

        student.Enrolments.Add(newEnrolment);
        course.Enrolments.Add(newEnrolment);

        Enrolments.Add(newEnrolment);   
    }

    private static void _seedEnrolmentMethod()
    {
        CreateEnrolment(Students.First(), Courses.Last());
        CreateEnrolment(Students.First(), Courses.First());
        CreateEnrolment(Students.Last(), Courses.Last());
        CreateEnrolment(Students.Last(), Courses.Last());
    }
}

//  PRACTICE 

