using minimalapi.api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using minimalapi.domain.db;
using minimalapi.domain.interfaces;
using minimalapi.test.mocks;

namespace minimalapi.test.helpers;

public class Setup
{

    public const string PORT = "5200";
    public static TestContext? TestContext = default;
    public static WebApplicationFactory<Startup>? Http = default;
    public static HttpClient? Client = default;

    public static void ClassInit(TestContext context)
    {
        Setup.TestContext = context;
        Setup.Http = new WebApplicationFactory<Startup>();
        Setup.Http = Setup.Http.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Register other necessary services for testing
                services.AddScoped<IVehicleService, VehicleServiceMock>();
            });            
        });


    }
}