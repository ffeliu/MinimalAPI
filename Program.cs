using minimalapi.domain.dto;
using minimalapi.domain.db;
using minimalapi.domain.entity;
using Microsoft.EntityFrameworkCore;
using minimalapi.domain.interfaces;
using minimalapi.domain.services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Minimal API Example",
        Version = "v1",
        Description = "Exemplo de API com Swagger e Minimal API"
    });
});

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    var result = adminService.Authenticate(loginDTO);

    if (result != null)
    {
        return Results.Ok();
    }

    return Results.Unauthorized();
});

app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    if (vehicleDTO == null)
    {
        return Results.BadRequest("Vehicle data is required.");
    }

    var vehicle = new Vehicle
    {
        Name = vehicleDTO.Name,
        Color = vehicleDTO.Color,
        Brand = vehicleDTO.Brand,
        Year = vehicleDTO.Year
    };

    vehicleService.CreateVehicle(vehicle);

    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
});

app.Run();

