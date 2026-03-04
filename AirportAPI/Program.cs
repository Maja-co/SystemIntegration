using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. TILFØJ DENNE LINJE: Fortæl programmet, at vi bruger Controllers
builder.Services.AddControllers(); 

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 2. TILFØJ DENNE LINJE: Fortæl programmet, at det skal finde din GateInfoController
app.MapControllers(); 

app.Run();