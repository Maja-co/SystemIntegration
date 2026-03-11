using TodoApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Denne linje er vigtig! Den fortæller projektet, at vi bruger Controllere
builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Denne linje aktiverer ruten (f.eks. /api/todo), så det virker i Postman
app.MapControllers();

app.Run();