
using System.Data.Common;
using minimalapi.domain.entity;

namespace minimalapi.test.mocks;


public class VehicleServiceMock : minimalapi.domain.interfaces.IVehicleService
{
    private static List<Vehicle> _vehicles = new List<Vehicle>{};

    public VehicleServiceMock()
    {
        _vehicles.Add(new Vehicle
        {
            Id = 1,
            Name = "Mock Vehicle",
            Brand = "Mock Brand",
            Color = "Red",
            Year = 2020
        });
    }

    public List<Vehicle> GetAllVehicles(int paging = 0, int pageSize = 10)
    {
        return _vehicles.Skip(paging * pageSize).Take(pageSize).ToList();
    }

    public Vehicle GetVehicleById(int id)
    {
        return _vehicles.Find(v => v.Id == id);
    }

    public Vehicle GetVehicleByName(string name)
    {
        return _vehicles.Find(v => v.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public int CreateVehicle(Vehicle vehicle)
    {
        // Mock implementation
        return 1; // Return a mock ID
    }

    public void UpdateVehicle(Vehicle vehicle)
    {
        // Mock implementation
    }

    public void DeleteVehicle(Vehicle vehicle)
    {
        // Mock implementation
    }
}