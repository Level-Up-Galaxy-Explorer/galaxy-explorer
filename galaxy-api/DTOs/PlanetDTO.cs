namespace galaxy_api.DTOs
{
    public class PlanetDTO
    {
        // public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Galaxy { get; set; } = string.Empty;
        public string PlanetType { get; set; } = string.Empty;
        public bool HasLife { get; set; }
        public string Coordinates { get; set; } = string.Empty;
    }
}
