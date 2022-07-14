using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransportInfo.Models.Entities;

#pragma warning disable CS8618, IDE1006
public record ParkingLocation
{
    [JsonPropertyName("id")]
    [Key] public int ID { get; set; }
    [JsonPropertyName("parkeringstilbyderNavn")]
    public string ParkingProviderName { get; set; }
    [JsonPropertyName("breddegrad")]
    public float Longitude { get; set; }
    [JsonPropertyName("lengdegrad")]
    public float Latitude { get; set; }
    [JsonPropertyName("Deactivation")]
    public Deactivation? Deactivated { get; set; }
    [JsonPropertyName("versjonsnummer")]
    public int VersionNumber { get; set; }
    [JsonPropertyName("navn")]
    public string Name { get; set; }
    [JsonPropertyName("adresse")]
    public string? Address { get; set; }
    [JsonPropertyName("postnummer")]
    public string ZipCode { get; set; }
    [JsonPropertyName("poststed")]
    public string ZipAreaName { get; set; }
    [JsonPropertyName("aktiveringstidspunkt")]
    public DateTime ActivationTime { get; set; }

    public ParkingLocationInfo ParkingLocationInfo { get; set; }

    public class Deactivation
    {
        [JsonPropertyName("deaktivertTidspunkt")]
        public DateTime Time { get; set; }

        public ParkingLocation ParkingLocation { get; set; }
        [Key] public int ParkingLocationID { get; set; }
    }
}
#pragma warning restore