using TransportInfo.Models;
using TransportInfo.Extensions;
using System.Text.Json;
using TransportInfo.Models.Entities;

namespace TransportInfo.Data;

public class ParkeringsRegisteretSeedAdapter : IParkeringsRegisteretAdapter
{
    public async Task<ParkingLocation[]?> GetParkingLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await DeserializeJsonAsync<ParkingLocation[]?>("seed/ParkingLocations.json", cancellationToken);
    }

    public async Task<ParkingProvider[]?> GetParkingProvidersAsync(CancellationToken cancellationToken = default)
    {
        return await DeserializeJsonAsync<ParkingProvider[]?>("seed/ParkingProviders.json", cancellationToken);
    }

    static async Task<T?> DeserializeJsonAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        using var fs = File.Open(AppContext.BaseDirectory + path, FileMode.Open);
        return await JsonSerializer.DeserializeAsync<T>(fs, cancellationToken: cancellationToken);
    }
}

public static class ParkingRegistrySeed
{
    static readonly IParkeringsRegisteretAdapter parkeringsRegisteretAdapter =
        new ParkeringsRegisteretSeedAdapter();

    public static void Initialize(ParkingRegistryContext context)
    {
        if (context.ParkingLocations.Any() is false)
        {
            InitializeParkingLocations(context);
        }

        if (context.ParkingProviders.Any() is false)
        {
            InitializeParkingProviders(context);
        }
    }

    private static void InitializeParkingLocations(ParkingRegistryContext context)
    {
        var data = parkeringsRegisteretAdapter.GetParkingLocationsAsync().ResolveBlocking();
        if (data is null) return;

        int i = 0;
        foreach (var entry in data)
        {
            entry.ParkingLocationInfo = new();
            if (i++ % 20 == 0)
            {
                entry.ParkingLocationInfo.ParkingLocation = entry;
                entry.ParkingLocationInfo.ParkingLocationID = entry.ID;
                entry.ParkingLocationInfo.ShowerType = ShowerType.Yes;
                entry.ParkingLocationInfo.VehicleType = VehicleType.Trailer;
            }
        }

        context.ParkingLocations.AddRange(data);
        context.SaveChanges();
    }

    private static void InitializeParkingProviders(ParkingRegistryContext context)
    {
        var data = parkeringsRegisteretAdapter.GetParkingProvidersAsync().ResolveBlocking();
        if (data is null) return;

        context.ParkingProviders.AddRange(data);
        context.SaveChanges();
    }
}