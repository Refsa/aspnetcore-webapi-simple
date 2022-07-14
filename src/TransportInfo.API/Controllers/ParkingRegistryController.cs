using Microsoft.AspNetCore.Mvc;
using TransportInfo.Models;
using TransportInfo.Models.Entities;

namespace TransportInfo.Controllers;

[ApiController]
[Route("api/v1/parking/")]
public class ParkingRegistryController : ControllerBase
{
    private readonly IParkingRegistryContext _context;
    private readonly ILogger<ParkingRegistryController> _logger;

    public ParkingRegistryController(ILogger<ParkingRegistryController> logger, IParkingRegistryContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("locations/all")]
    [ProducesResponseType(typeof(IEnumerable<ParkingLocationDTO>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<ParkingLocationDTO>> GetParkingLocations()
    {
        var query = _context.ParkingLocations.AsQueryable();
        return await Task.FromResult(query.Select(e => ToParkingLocationDTO(e)));
    }

    [HttpGet("locations")]
    [ProducesResponseType(typeof(IEnumerable<ParkingLocationDTO>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<ParkingLocationDTO>> GetParkingLocations(VehicleType vehicleType, ShowerType showerType)
    {
        var query = _context.ParkingLocations.Where(e =>
            e.ParkingLocationInfo != null &&
            e.ParkingLocationInfo.VehicleType == vehicleType &&
            e.ParkingLocationInfo.ShowerType == showerType);

        return await Task.FromResult(query.Select(e => ToParkingLocationDTO(e)));
    }

    [HttpGet("locations/{id:int}")]
    [ProducesResponseType(typeof(ParkingLocationDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingLocationDTO>> GetParkingLocations(int id)
    {
        var query = await _context.ParkingLocations.FindAsync(id);
        if (query is null)
        {
            _logger.LogWarning(new EventId(4321), "Couldnt find ParkingLocation of id {@id}", id);
            return NotFound();
        }

        var parkingLocationInfo = await _context.ParkingLocationInfo.FindAsync(id);
        if (parkingLocationInfo is not null)
        {
            query.ParkingLocationInfo = parkingLocationInfo;
        }
        else
        {
            _logger.LogWarning(
                new EventId(1234, "ParkingLocationNotFound"),
                "No ParkingLocationInfo found for {@locationId}", id);
        }

        return ToParkingLocationDTO(query);
    }

    [HttpGet("locations/{id:int}/info")]
    [ProducesResponseType(typeof(ParkingLocationInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingLocationInfoDTO>> GetParkingLocationInfo(int id)
    {
        var query = await _context.ParkingLocations.FindAsync(id);
        if (query is null) return NotFound("ParkingLocation doesnt exist");

        var facilities = await _context.ParkingLocationInfo!.FindAsync(id);
        if (facilities is null) return NotFound("ParkingLocationInfo doesnt exist");

        return ToParkingLocationInfoDTO(facilities);
    }

    [HttpGet("locations/{id:int}/parkingProvider")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingProviderDTO>> GetParkingLocationProvider(int id)
    {
        var location = await _context.ParkingLocations.FindAsync(id);
        if (location is null) return NotFound();

        var provider = _context.ParkingProviders
            .Where(e => e.Name == location.ParkingProviderName)
            .FirstOrDefault();
        if (provider is null) return NotFound();

        return ToParkingProviderDTO(provider);
    }

    [HttpGet("providers")]
    [ProducesResponseType(typeof(IEnumerable<ParkingProviderDTO>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<ParkingProviderDTO>> GetParkingProviders()
    {
        return await Task.FromResult(
            _context.ParkingProviders!.Select(e => ToParkingProviderDTO(e))
        );
    }

    [HttpGet("providers/{id:int}")]
    [ProducesResponseType(typeof(ParkingProviderDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParkingProviderDTO>> GetParkingProvider(int id)
    {
        var provider = await _context.ParkingProviders.FindAsync(id);
        if (provider is null) return NotFound();

        return ToParkingProviderDTO(provider);
    }


    static ParkingLocationInfoDTO ToParkingLocationInfoDTO(ParkingLocationInfo pli)
    {
        return new()
        {
            DrinkingWater = pli.DrinkingWater,
            GarbageDisposal = pli.GarbageDisposal,
            PowerOutlet = pli.PowerOutlet,
            ShowerType = pli.ShowerType,
            VehicleType = pli.VehicleType,
        };
    }

    static ParkingLocationDTO ToParkingLocationDTO(ParkingLocation parkingLoc)
    {
        return new()
        {
            ID = parkingLoc.ID,
            Latitude = parkingLoc.Latitude,
            Longitude = parkingLoc.Longitude,
            Name = parkingLoc.Name,
            ParkingLocationInfo = parkingLoc.ParkingLocationInfo is null ? null :
                ToParkingLocationInfoDTO(parkingLoc.ParkingLocationInfo),
        };
    }

    static ParkingProviderDTO ToParkingProviderDTO(ParkingProvider parkingProvider)
    {
        return new()
        {
            ID = parkingProvider.ID,
            Name = parkingProvider.Name ?? "",
            OrgNumber = parkingProvider.OrganisationNumber,
        };
    }
}