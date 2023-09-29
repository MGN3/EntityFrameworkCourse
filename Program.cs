using EntityFrameworkCourse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

//InMemoryDatabase is useful to test purposes.
//builder.Services.AddDbContext<TaskContext>(context => context.UseInMemoryDatabase("TasksDB"));

builder.Services.AddSqlServer<TaskContext>(builder.Configuration.GetConnectionString("cnTaskDb"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/dbconnection", async ([FromServices] TaskContext dbContext) => {
	dbContext.Database.EnsureCreated();
	return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
});

app.Run();

