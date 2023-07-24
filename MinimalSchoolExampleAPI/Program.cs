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

app.MapGet("/students/not-passing", (int? courseId) =>
{
    HashSet<Course> courses = new HashSet<Course>();

    // if course ID is not null 
    if (courseId != null)
    {
        try
        {
            Course course = InternalDatabase.Courses.First(c => c.CourseNumber == courseId);

            courses.Add(course);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(ex.Message);
        }
    } else
    {
        courses = InternalDatabase.Courses;
    }

    HashSet<Student> StudentsNotPassing = InternalDatabase.Students.Where(s =>
    {
        return s.Enrolments.Any(e => e.Grade < e.Course.PassingGrade 
            && courses.Contains(e.Course)
        );
    }).ToHashSet();

    return Results.Ok(StudentsNotPassing);
});

// COURSES
app.MapGet("courses/averagestudentcount", () => {
    return InternalDatabase.Courses.Average(c => c.Enrolments.Count);
});

app.MapGet("courses/compare", (int courseOne, int courseTwo) =>
{
    try
    {
        Course firstCourse = InternalDatabase.Courses.First(c => c.CourseNumber == courseOne);
        Course secondCourse = InternalDatabase.Courses.First(c => c.CourseNumber == courseTwo);

        HashSet<Course> courses = new HashSet<Course> { firstCourse, secondCourse };

        int sumOfStudents = courses.Sum(c => c.Enrolments.Count);

        return Results.Ok(new {
            Courses = courses,
            TotalStudents = sumOfStudents
        });

    } catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    } 
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("courses/search/year-range", (int? before, int? after) =>
{
    if (before == null && after == null)
    {
        return Results.BadRequest("At least one value must be provided for before and after parameters.");
    }

    if(before == null)
    {
        before = Int32.MaxValue;
    }

    if(after == null)
    {
        after = Int32.MinValue;
    }

    if (before < after)
    {
        return Results.BadRequest("Cannot request courses with an After date greater than a Before date.");
    }

    HashSet<Course> coursesInRange = InternalDatabase.Courses.Where(c => c.Year <= before && c.Year >= after).ToHashSet();

    return Results.Ok(coursesInRange);
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

    public static void CreateCourse(string title, int year)
    {
        try
        {
            if (!String.IsNullOrEmpty(title))
            {
                Course course = new Course(_pkCount++, title, year);
                Courses.Add(course);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Title must have a value.");
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static void _seedCourseMethod()
    {
        CreateCourse("Intro to Philosophy", 2023);
        CreateCourse("Advanced Utilitarianism", 2023);
        CreateCourse("Descartes for Dummies", 2024);
        CreateCourse("Future Anthropology", 2040);
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
        CreateEnrolment(Students.Last(), Courses.First());

        Enrolments.Last().Grade = 92;
        Enrolments.First().Grade = 47;
    }
}


// get all of the courses that fall between two specified years in a range. If no argument is given for either value (before or after), it should show all courses before a value, or all courses after

// get all students whose names start with a given search value

// get all courses whose numbers start with a given search value

// provide each course a Passing Grade member, and get a list of students in a course not passing that Course. If no course search value is provided, show all students that are not passing at least one of their courses
