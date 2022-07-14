using TransportInfo.Data;
using TransportInfo.Models;
using TransportInfo.Models.Entities;

namespace TransportInfo.Services;

public class ParkeringsRegisteretService : BackgroundService
{
    const int RunInterval = 60 * 60 * 24 * 1000;

    readonly ILogger<ParkeringsRegisteretService> _logger;
    readonly IServiceProvider _serviceProvider;

    public ParkeringsRegisteretService(
        IServiceProvider serviceProvider,
        ILogger<ParkeringsRegisteretService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            await Task.Delay(RunInterval, stoppingToken);
            
            _logger.LogInformation("Updating data...");

            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IParkingRegistryContext>();
                var adapter = scope.ServiceProvider.GetRequiredService<IParkeringsRegisteretAdapter>();

                await UpdateParkingProviders(dbContext, adapter, stoppingToken);
                await UpdateParkingLocations(dbContext, adapter, stoppingToken);
            }

            _logger.LogInformation("Update completed");
        }
    }

    async Task UpdateParkingLocations(
        IParkingRegistryContext dbContext,
        IParkeringsRegisteretAdapter adapter,
        CancellationToken stoppingToken)
    {
        var parkingLocations = await adapter.GetParkingLocationsAsync(stoppingToken);
        if (parkingLocations is null)
        {
            _logger.LogCritical("Failed to fetch ParkingLocations from external API: Parkeringsregisteret");
        }
        else
        {
            foreach (var item in parkingLocations)
            {
                if (await dbContext.ParkingLocations.FindAsync(new object?[] { item.ID }, cancellationToken: stoppingToken)
                        is ParkingLocation existing)
                {
                    if (existing != item)
                    {
                        existing.ParkingProviderName = item.ParkingProviderName;
                        existing.Longitude = item.Longitude;
                        existing.Latitude = item.Latitude;
                        existing.VersionNumber = item.VersionNumber;
                        existing.Name = item.Name;
                        existing.Address = item.Address;
                        existing.ZipCode = item.ZipCode;
                        existing.ZipAreaName = item.ZipAreaName;
                        existing.ActivationTime = item.ActivationTime;
                    }
                }
                else
                {
                    dbContext.ParkingLocations.Add(item);
                }

                if (dbContext.ParkingLocationInfo.Find(item.ID) is null)
                {
                    dbContext.ParkingLocationInfo.Add(new ParkingLocationInfo()
                    {
                        ParkingLocationID = item.ID,
                        ParkingLocation = item
                    });
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Updated ParkingLocations");
        }
    }

    async Task UpdateParkingProviders(
        IParkingRegistryContext dbContext,
        IParkeringsRegisteretAdapter adapter,
        CancellationToken stoppingToken)
    {
        var parkingProviders = await adapter.GetParkingProvidersAsync(stoppingToken);
        if (parkingProviders is null)
        {
            _logger.LogCritical("Failed to fetch ParkingProviders from external API: Parkeringsregisteret");
        }
        else
        {
            foreach (var item in parkingProviders)
            {
                if (await dbContext.ParkingProviders.FindAsync(new object?[] { item.ID }, cancellationToken: stoppingToken)
                        is ParkingProvider existing)
                {
                    if (existing != item)
                    {
                        existing.ID = item.ID;
                        existing.OrganisationNumber = item.OrganisationNumber;
                        existing.Name = item.Name;
                    }
                }
                else
                {
                    dbContext.ParkingProviders.Add(item);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("Updated ParkingProviders");
        }
    }
}