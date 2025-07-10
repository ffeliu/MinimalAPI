using minimalapi.domain.dto;
using minimalapi.domain.db;
using minimalapi.domain.entity;
using Microsoft.EntityFrameworkCore;
using minimalapi.domain.interfaces;
using minimalapi.domain.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Drawing.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Minimal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Digite o token JWT no campo abaixo.\n\nExemplo: Bearer eyJhbGciOiJIUzI1..."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });    
});

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

string GetTokenJwt(Admin admin)
{
    var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

    var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey,
        Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

    var claims = new List<System.Security.Claims.Claim>
    {
        new Claim("Email", admin.Email),
        new Claim("Profile", admin.Profile),
        new Claim(ClaimTypes.Role, admin.Profile),
    };

    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        claims: claims,
        issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
        expires: DateTime.Now.AddMinutes(Convert.ToDouble(builder.Configuration["Jwt:DurationInMinutes"])),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
{
    var result = adminService.Authenticate(loginDTO);

    if (result != null)
    {
        string token = GetTokenJwt(result);
        return Results.Ok(new { Token = token });
    }

    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Authentication");

app.MapPost("/admin", ([FromBody] AdministradorDTO adminDTO, IAdminService adminService) =>
{
    try
    {
        var admin = new Admin
        {
            Name = adminDTO.Name,
            Email = adminDTO.Email,
            Password = adminDTO.Password,
            Profile = adminDTO.Profile
        };

        adminService.CreateAdmin(admin);
        
        return Results.Created($"/admin/{admin.Id}", admin);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("Admin").RequireAuthorization();




app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    try
    {
        var vehicle = new Vehicle
        {
            Name = vehicleDTO.Name,
            Color = vehicleDTO.Color,
            Brand = vehicleDTO.Brand,
            Year = vehicleDTO.Year
        };

        vehicleService.CreateVehicle(vehicle);
        
        return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("Vehicles").RequireAuthorization(new AuthorizeAttribute
{
    Roles = "Admin"
});

app.MapGet("/vehicles", ([FromQuery] int paging, [FromQuery] int pageSize, IVehicleService vehicleService) =>
{
    return vehicleService.GetAllVehicles(paging, pageSize);
}).WithTags("Vehicles").RequireAuthorization();

app.MapGet("/vehicles/{id:int}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    try
    {
        var vehicle = vehicleService.GetVehicleById(id);
        return Results.Ok(vehicle);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound($"Vehicle with ID {id} not found.");
    }
}).WithTags("Vehicles").RequireAuthorization();

app.MapPut("/vehicles/{id:int}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    try
    {
        var result = vehicleService.GetVehicleById(id);

        if (result == null)
        {
            return Results.NotFound($"Vehicle with ID {id} not found.");
        }

        result.Name = vehicleDTO.Name;
        result.Color = vehicleDTO.Color;
        result.Brand = vehicleDTO.Brand;
        result.Year = vehicleDTO.Year;

        vehicleService.UpdateVehicle(result);

        return Results.NoContent();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).WithTags("Vehicles").RequireAuthorization();

app.MapDelete("/vehicles/{id:int}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    try
    {
        var vehicle = vehicleService.GetVehicleById(id);

        if (vehicle == null)
        {
            return Results.NotFound($"Vehicle with ID {id} not found.");
        }

        vehicleService.DeleteVehicle(vehicle);
        return Results.NoContent();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound($"Vehicle with ID {id} not found.");
    }
}).WithTags("Vehicles").RequireAuthorization();

app.Run();

