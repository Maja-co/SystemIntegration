using Microsoft.EntityFrameworkCore;
using BlogPostApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrer DbContext med InMemory
builder.Services.AddDbContext<BlogPostData>(opt =>
    opt.UseInMemoryDatabase("BlogDb"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();