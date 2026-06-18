var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    return Results.Ok(new { message = "API is connected to the network!" });
})
.WithName("GetWeatherForecast");

app.Run();