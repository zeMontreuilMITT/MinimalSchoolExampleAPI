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
    context.Students.Add(new Student { FirstName = "Fido", LastName = "Scooby" });
    context.Students.Add(new Student { FirstName = "Babs", LastName = "Ratchett" });
    context.Students.Add(new Student { FirstName = "Stephen", LastName = "King" });

    context.SaveChanges();
});

app.MapGet("/students", (SchoolExampleContext context) =>
{
    return context.Students.ToHashSet();
});

app.Run();

