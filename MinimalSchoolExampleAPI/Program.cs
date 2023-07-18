using MinimalSchoolExampleAPI.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// respond to GET request on "/hello" directory

app.MapGet("/students", () =>
{
    return Results.Ok(InternalDatabase.Students);
});

app.Run();


static class InternalDatabase
{
    private static int _pkCount = 1;

    public static HashSet<Student> Students { get; set; }

    static InternalDatabase()
    {
        Students = new HashSet<Student>();

        _seedMethodStudents();
    }

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
}

