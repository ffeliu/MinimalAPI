using minimalapi.domain.dto;
using minimalapi.domain.db;
using minimalapi.domain.entity;
using Microsoft.EntityFrameworkCore;
using minimalapi.domain.interfaces;
using minimalapi.domain.services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => 
{
    var result = adminService.Authenticate(loginDTO);

    // if (loginDTO.Username == "admin" && loginDTO.Password == "password")
    // {
    //     return Results.Ok("Login successful");
    // }

    return Results.Unauthorized();
});

app.Run();

