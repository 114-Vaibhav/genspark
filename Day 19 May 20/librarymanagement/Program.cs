using Microsoft.EntityFrameworkCore;
using librarymanagement.DataAccessLib.Contexts;
using librarymanagement.DataAccessLib;

using librarymanagement.ModelLib;
using librarymanagement.BusinessLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// OpenAPI / Swagger
builder.Services.AddOpenApi();

// Database Configuration
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=librarymanagementwithwebapi;Username=vaibhavgupta;Password=8098"));

builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<MemberRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Library Management API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();