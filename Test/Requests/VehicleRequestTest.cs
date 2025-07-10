using minimalapi.domain.dto;
using minimalapi.domain.entity;
using minimalapi.test.helpers;

namespace Test.Domain.Entity;

[TestClass]
public class VehicleRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Setup.ClassInit(context);
        Setup.Client = Setup.Http!.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.Http?.Dispose();
        Setup.Client?.Dispose();
        Setup.TestContext = null;
    }

    [TestMethod]
    public void GetVehicleTest()
    {
        // Arrange
        var vehicleId = 1; // Assuming a vehicle with ID 1 exists in the mock

        // Act
        var response = Setup.Client!.GetAsync($"/vehicles/{vehicleId}").Result;

        // Assert
        Assert.IsTrue(response.IsSuccessStatusCode, "Response should be successful");

        var json = response.Content.ReadAsStringAsync().Result;
        var vehicle = System.Text.Json.JsonSerializer.Deserialize<Vehicle>(json,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.IsNotNull(vehicle, "Vehicle should not be null");
        Assert.AreEqual(vehicleId, vehicle.Id, "Vehicle ID should match the requested ID");
    }
}