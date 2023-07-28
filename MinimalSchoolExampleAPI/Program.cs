using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MinimalSchoolExampleAPI.Data;
using MinimalSchoolExampleAPI.Models;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SchoolContextConnection");

builder.Services.AddDbContext<SchoolExampleContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.Configure<JsonOptions>(options =>
{
   options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

app.MapPost("/students/seed", (SchoolExampleContext context) =>
{
    if(!context.Students.Any())
    {
        context.Students.Add(new Student { FirstName = "Fido", LastName = "Scooby" });
        context.Students.Add(new Student { FirstName = "Babs", LastName = "Ratchett" });
        context.Students.Add(new Student { FirstName = "Stephen", LastName = "King" });
    } else
    {
        return Results.Conflict();
    }

    context.SaveChanges();
    return Results.Created("/students", context.Students.ToList());
});

app.MapPost("/courses/seed", (SchoolExampleContext context) =>
{
    if(context.Courses.Any()) {
        return Results.Conflict();
    }
    HashSet<Course> courses = new HashSet<Course> {
        new Course("Intro to Seed Methods", 2023),
        new Course("Hash Sets for Dummies", 2022),
        new Course("How to Use a Computer", 1998, 90)
    };

    foreach(Course c in courses)
    {
        context.Courses.Add(c);
    }

    context.SaveChanges();
    return Results.Created("/courses", context.Courses.ToHashSet());
});

app.MapPost("/enrolments/seed", (SchoolExampleContext context) =>
{
    if (context.Enrolments.Any())
    {
        return Results.Conflict();
    }

    HashSet<Enrolment> newEnrolments = new HashSet<Enrolment>
    {
        new Enrolment(context.Students.First(), context.Courses.First()),
        new Enrolment(context.Students.OrderByDescending(s => s.Id).First(), context.Courses.First()),
        new Enrolment(context.Students.OrderByDescending(s => s.Id).First(), context.Courses.OrderByDescending(c => c.Id).First())
    };

    foreach(Enrolment e in newEnrolments)
    {
        context.Enrolments.Add(e);
    }

    context.SaveChanges();

    return Results.Created("/enrolments", context.Enrolments.ToHashSet());
});

app.MapGet("/students", (SchoolExampleContext context) =>
{
    return context.Students
        .Include(s => s.Enrolments)
        .ThenInclude(e => e.Course)
            .ToHashSet();
});

app.MapPost("/students", (SchoolExampleContext db, Student student) =>
{
    db.Students.Add(student);
    db.SaveChanges();
});

app.Run();
// add a seed method to create at least 3 courses and 3 enrolments
// create a method that Gets a Student, and their Enrolments, and the Courses in those enrolments, using the .Include method

