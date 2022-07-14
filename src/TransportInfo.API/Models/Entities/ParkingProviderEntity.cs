using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransportInfo.Models.Entities;

#pragma warning disable CS8618, IDE1006
public class ParkingProvider
{
    [JsonPropertyName("id"), Key]
    public int ID { get; set; }
    [JsonPropertyName("organisasjonsnummer")]
    public string OrganisationNumber { get; set; }
    [JsonPropertyName("navn")]
    public string? Name { get; set; }
}
#pragma warning restore