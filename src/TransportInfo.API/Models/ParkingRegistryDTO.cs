using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TransportInfo.Models.Entities;

namespace TransportInfo.Models;

#pragma warning disable CS8618
public class ParkingLocationDTO
{
    public int ID { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
    public string Name { get; set; }
    public ParkingLocationInfoDTO? ParkingLocationInfo { get; set; }
}

public class ParkingProviderDTO
{
    public int ID { get; set; }
    public string OrgNumber { get; set; }
    public string Name { get; set; }
}

public class ParkingLocationInfoDTO
{
    public VehicleType VehicleType { get; set; }
    public ShowerType ShowerType { get; set; }
    public bool DrinkingWater { get; set; }
    public bool PowerOutlet { get; set; }
    public GarbageDisposalType GarbageDisposal { get; set; }
}
#pragma warning restore