namespace galaxy_api.Models
{
    public class Planet
    {
        public string Name { get; set; } = string.Empty;
        public string Galaxy { get; set; } = string.Empty;
        public string PlanetType { get; set; } = string.Empty;
        public bool HasLife { get; set; }
        public string Coordinates { get; set; } = string.Empty;
    }
}
