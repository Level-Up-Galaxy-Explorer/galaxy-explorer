using System.Text.Json.Serialization;

namespace galaxy_cli.DTO.Galaxy
{
    public class GalaxyDTO
    {
        [JsonPropertyName("galaxy_Id")]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("galaxy_Type")]
        public string GalaxyType { get; set; } = string.Empty;

        [JsonPropertyName("distance_From_Earth")]
        public double Distance_From_Earth { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}