using Auth0.AspNetCore.Authentication.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0ApiAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"]!;
    options.Audience = builder.Configuration["Auth0:Audience"]!;
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/public", () =>
    Results.Ok(new { Message = "This endpoint is public" }));

app.MapGet("/api/private", () =>
    Results.Ok(new { Message = "This endpoint requires authentication" }))
    .RequireAuthorization();

app.Run();