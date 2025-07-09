using System.Data.Entity;
using Microsoft.AspNetCore.Http.HttpResults;
using minimalapi.domain.db;
using minimalapi.domain.entity;
using minimalapi.domain.interfaces;

namespace minimalapi.domain.services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _db;
    public VehicleService(AppDbContext dbContext)
    {
        _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public int CreateVehicle(Vehicle vehicle)
    {
        validateVehicle(vehicle);

        _db.Add(vehicle);
        _db.SaveChanges();

        return vehicle.Id;
    }

    private static void validateVehicle(Vehicle vehicle)
    {
        var validationErrors = new ValidationErrors();

        if (vehicle == null)
            validationErrors.Messages.Add("Vehicle cannot be null.");

        if (string.IsNullOrEmpty(vehicle.Name))
            validationErrors.Messages.Add("Vehicle name cannot be null or empty.");

        if (string.IsNullOrEmpty(vehicle.Brand))
            validationErrors.Messages.Add("Vehicle brand cannot be null or empty.");

        if (string.IsNullOrEmpty(vehicle.Color))
            validationErrors.Messages.Add("Vehicle color cannot be null or empty.");

        if (vehicle.Year < 1950 || vehicle.Year > DateTime.Now.Year)
            validationErrors.Messages.Add($"Vehicle year must be between 1950 and {DateTime.Now.Year + 1}.");

        if (validationErrors.Messages.Any())
        {
            throw new ArgumentException("Invalid vehicle data: " + string.Join("; ", validationErrors.Messages),
            nameof(vehicle));
        }
    }

    public void DeleteVehicle(Vehicle vehicle)
    {
        _db.Remove(vehicle);
        _db.SaveChanges();
    }

    public List<Vehicle> GetAllVehicles(int paging = 0, int pageSize = 10)
    {
        if (paging < 0 || pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException("Paging must be non-negative and pageSize must be positive.");
        }

        // Implement pagination logic
        var vehicles = _db.Vehicles
            .OrderBy(v => v.Id)
            .Skip(paging * pageSize)
            .Take(pageSize)
            .ToList();


        return vehicles;
    }

    public Vehicle GetVehicleById(int id)
    {
        var vehicle = _db.Vehicles.Find(id);

        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehicle with ID {id} not found.");
        }

        return vehicle;
    }

    public Vehicle GetVehicleByName(string name)
    {
        var vehicle = _db.Vehicles.FirstOrDefault(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (vehicle == null)
        {
            throw new KeyNotFoundException($"Vehicle with name '{name}' not found.");
        }

        return vehicle;
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        validateVehicle(vehicle);

        _db.Update(vehicle);    
        _db.SaveChanges();
    }
}