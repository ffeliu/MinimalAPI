
IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<minimalapi.api.Startup>();
        });    
}


CreateHostBuilder(args).Build().Run();

