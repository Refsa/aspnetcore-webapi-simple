using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using TransportInfo.Data;

namespace TransportInfo.API.Tests;

public class TransportInfoFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("integrationsettings.json")
                .Build();

            config.AddConfiguration(Configuration);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<IParkeringsRegisteretAdapter, ParkeringsRegisteretSeedAdapter>();
        });
        
        base.ConfigureWebHost(builder);
    }
}

public class ParkingRegistryControllerTests : IClassFixture<TransportInfoFactory>
{
    const string BasePath = "api/v1/parking/";

    readonly TransportInfoFactory _factory;

    public ParkingRegistryControllerTests(TransportInfoFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("locations/all")]
    [InlineData("locations")]
    [InlineData("providers")]
    [InlineData("locations/225")]
    [InlineData("locations/225/info")]
    [InlineData("locations/225/parkingProvider")]
    [InlineData("providers/21")]
    public async void GET_retreives_parking_locations_with_OK(string endpoint)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(BasePath + endpoint);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("locations/-1")]
    [InlineData("locations/-1/info")]
    [InlineData("locations/-1/parkingProvider")]
    [InlineData("providers/-1")]
    public async void GET_retreives_parking_locations_with_NotFound(string endpoint)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(BasePath + endpoint);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}