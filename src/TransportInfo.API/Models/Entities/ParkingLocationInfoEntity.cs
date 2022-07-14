using System.ComponentModel.DataAnnotations;

namespace TransportInfo.Models.Entities;

#pragma warning disable CS8618
public record ParkingLocationInfo
{
    public VehicleType VehicleType { get; set; }
    public ShowerType ShowerType { get; set; }
    public bool DrinkingWater { get; set; }
    public bool PowerOutlet { get; set; }
    public GarbageDisposalType GarbageDisposal { get; set; }

    public ParkingLocation ParkingLocation { get; set; }
    [Key] public int ParkingLocationID { get; set; }
}

public enum VehicleType
{
    Unknown = 0,
    Car,
    Bus,
    Trailer,
    Truck,
}

public enum ShowerType
{
    Unknown = 0,
    Yes,
    ForMobilityImpaired,
    No,
}

public enum GarbageDisposalType
{
    Unknown = 0,
    Bin,
    Container,
}
#pragma warning restore