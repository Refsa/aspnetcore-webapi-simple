using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransportInfo.Models.Entities;

namespace TransportInfo.Models;

#pragma warning disable CS8618
public interface IParkingRegistryContext
{
    DbSet<ParkingLocation> ParkingLocations { get; set; }
    DbSet<ParkingProvider> ParkingProviders { get; set; }
    DbSet<ParkingLocationInfo> ParkingLocationInfo { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ParkingRegistryContext : DbContext, IParkingRegistryContext
{
    public DbSet<ParkingLocation> ParkingLocations { get; set; }
    public DbSet<ParkingProvider> ParkingProviders { get; set; }
    public DbSet<ParkingLocationInfo> ParkingLocationInfo { get; set; }

    public ParkingRegistryContext(DbContextOptions<ParkingRegistryContext> contextOpts)
        : base(contextOpts)
    {

    }
}
#pragma warning restore