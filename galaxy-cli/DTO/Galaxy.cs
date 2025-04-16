using System.Text.Json.Serialization;

namespace galaxy_cli.DTO
{
    public class GalaxyDTO
    {
        public int Galaxy_Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("galaxy_type_name")]
        public string Galaxy_Type_Name { get; set; } = string.Empty;

        public double Distance_From_Earth { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}