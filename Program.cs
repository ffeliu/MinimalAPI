using minimalapi.domain.dto;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => 
{
    if (loginDTO.Username == "admin" && loginDTO.Password == "password")
    {
        return Results.Ok("Login successful");
    }

    return Results.Unauthorized();
});

app.Run();

