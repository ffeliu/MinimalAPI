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

namespace minimalapi.api;

public class Startup
{
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _durationInMinutes;
    private readonly string _connectionString;

    public Startup(IConfiguration configuration)
    {
        _jwtKey = configuration["Jwt:Key"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
        _durationInMinutes = configuration["Jwt:DurationInMinutes"];
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IConfiguration Configuration { get; }

    string GetTokenJwt(Admin admin)
    {
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_jwtKey));

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
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_durationInMinutes)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();



        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
            {
                var result = adminService.Authenticate(loginDTO);

                if (result != null)
                {
                    string token = GetTokenJwt(result);
                    return Results.Ok(new { Token = token });
                }

                return Results.Unauthorized();
            }).AllowAnonymous().WithTags("Authentication");

            endpoints.MapPost("/admin", ([FromBody] AdministradorDTO adminDTO, IAdminService adminService) =>
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

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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
            }).WithTags("Vehicles");

            endpoints.MapGet("/vehicles", ([FromQuery] int paging, [FromQuery] int pageSize, IVehicleService vehicleService) =>
            {
                return vehicleService.GetAllVehicles(paging, pageSize);
            }).WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id:int}", ([FromRoute] int id, IVehicleService vehicleService) =>
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
            }).WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id:int}", ([FromRoute] int id, [FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
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

            endpoints.MapDelete("/vehicles/{id:int}", ([FromRoute] int id, IVehicleService vehicleService) =>
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

        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
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

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddAuthentication(option =>
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
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtKey))
            };
        });

        services.AddAuthorization();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(_connectionString));

    }
}