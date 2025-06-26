using minimalapi.domain.dto;
using minimalapi.domain.entity;

namespace minimalapi.domain.interfaces;

public interface IVehicleService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paging"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    List<Vehicle> GetAllVehicles(int paging = 0, int pageSize = 10);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Vehicle GetVehicleById(int id);

    /// <summary>
    /// Gets a vehicle by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Vehicle GetVehicleByName(string name);

    /// <summary>
    /// Creates a new vehicle.
    /// </summary>
    /// <param name="vehicle"></param>
    /// <returns>Returns the ID of the created vehicle.</returns>
    int CreateVehicle(Vehicle vehicle);

    /// <summary>
    /// Updates an existing vehicle by its ID. 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="vehicle"></param>
    void UpdateVehicle(Vehicle vehicle);

    /// <summary>
    /// Deletes a vehicle by its ID.
    /// </summary>
    /// <param name="id"></param>
    void DeleteVehicle(Vehicle vehicle);    
}