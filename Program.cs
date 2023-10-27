using EntityFrameworkCourse;
using EntityFrameworkCourse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

//InMemoryDatabase is useful to test purposes.
//builder.Services.AddDbContext<TaskContext>(context => context.UseInMemoryDatabase("TasksDB"));

builder.Services.AddSqlServer<TaskContext>(builder.Configuration.GetConnectionString("cnTaskDb"));

var app = builder.Build();

/*
 * Creates the endpoint http://localhost:5207
 * A request to this endpoint will show Hello World!
 */
//app.MapGet("/", () => "Hello World!");

/*
 * The compiler will generate a constructor with no parameters automatically if a constructort has not been created.
 */
var newCategory = new Category {
	CategoryId = Guid.NewGuid(),
	Name = "Test category",
	Description = "Test description..."
};


var newTask = new EntityFrameworkCourse.Models.Task {
	TaskId = Guid.NewGuid(),
	CategoryId = newCategory.CategoryId,
	Title = "Task title",
	Description = "Task description!!!",
	TaskPriority = Priority.High,
	TaskCreated = DateTime.Now
};

app.MapGet("/dbconnection", async ([FromServices] TaskContext dbContext) => {
	dbContext.Database.EnsureCreated();

	return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
});

app.MapPost("/api/post", async ([FromServices] TaskContext dbContext, [FromBody] EntityFrameworkCourse.Models.Task task) => {
	//Two ways, create the task object here or create it in an outside variable like below.
	task.TaskId = Guid.NewGuid();
	task.TaskCreated = DateTime.Now;

	//Two await ways to add(newTask outside variable, task local variable.S
	await dbContext.AddAsync(newTask);
	//await dbContext.Tasks.AddAsync(newtask);

	await dbContext.SaveChangesAsync();
	return Results.Ok();
});

app.MapPost("/api/post2", async ([FromServices] TaskContext dbContext) => {

	dbContext.Categories.Add(newCategory);
	dbContext.SaveChanges();

	dbContext.Tasks.Add(newTask); // Agregar un nuevo usuario a la base de datos
	dbContext.SaveChanges(); // Guardar los cambios en la base de datos

	var test1 = dbContext.Tasks.ToList();
	test1.ForEach(task => Console.WriteLine(task.ToString()));
	return Results.Ok("SQL Server Database created: " + dbContext.Database.IsSqlServer());
});

var resultadosConsulta = app.MapGet("/api/get/where", async ([FromServices] TaskContext dbContext) => {
	var results = Results.Ok(dbContext.Tasks.Where(p => p.TaskPriority == Priority.High));

	return results;
});

app.MapPut("/api/update/{id}", async ([FromServices] TaskContext dbContext, [FromBody] EntityFrameworkCourse.Models.Task task, [FromRoute] Guid id) => {
	IResult result;

	try {
		var currentTask = dbContext.Tasks.Find(id);

		if (currentTask != null) {
			currentTask.CategoryId = task.CategoryId;
			currentTask.Title = task.Title;
			currentTask.Description = task.Description;
			currentTask.TaskPriority = task.TaskPriority;

			await dbContext.SaveChangesAsync();

			result = Results.Ok();
		} else {
			result = Results.NotFound();
		}
	} catch (Exception ex) {
		// Manejo de la excepción, puedes registrarla o devolver una respuesta de error personalizada
		result = Results.NotFound();
		Console.WriteLine(ex.ToString());
	}

	return result;
});

app.Run();

