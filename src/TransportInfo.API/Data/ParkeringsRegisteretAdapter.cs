using System.Net.Http.Headers;
using System.Text.Json;
using TransportInfo.Models.Entities;

namespace TransportInfo.Data;

public interface IParkeringsRegisteretAdapter
{
    Task<ParkingLocation[]?> GetParkingLocationsAsync(CancellationToken cancellationToken = default);
    Task<ParkingProvider[]?> GetParkingProvidersAsync(CancellationToken cancellationToken = default);
}

public class ParkeringsRegisteretAdapter : IParkeringsRegisteretAdapter
{
    const string BaseAddress = "https://www.vegvesen.no/ws/no/vegvesen/veg/parkeringsomraade/parkeringsregisteret/";
    const string ParkeringsomraadeEndpoint = "v1/parkeringsomraade";
    const string ParkeringstlbyderEndpoint = "v1/parkeringstilbyder";

    public async Task<ParkingLocation[]?> GetParkingLocationsAsync(CancellationToken cancellationToken = default)
    {
        var content = await GetJsonAsync(ParkeringsomraadeEndpoint, cancellationToken);
        if (content is not null)
        {
            return await content.ReadFromJsonAsync<ParkingLocation[]>(cancellationToken: cancellationToken);
        }

        return null;
    }

    public async Task<ParkingProvider[]?> GetParkingProvidersAsync(CancellationToken cancellationToken = default)
    {
        var content = await GetJsonAsync(ParkeringstlbyderEndpoint, cancellationToken);
        if (content is not null)
        {
            return await content.ReadFromJsonAsync<ParkingProvider[]>(cancellationToken: cancellationToken);
        }

        return null;
    }

    static async Task<HttpContent?> GetJsonAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();

        client.BaseAddress = new Uri(BaseAddress);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        var response = await client.GetAsync(endpoint, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return response.Content;
        }

        return null;
    }
}