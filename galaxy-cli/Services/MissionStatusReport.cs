namespace galaxy_api.DTOs
{
    public readonly struct MissionStatusReport
    {
        public string MissionType { get; init; }
        public string Status { get; init; }
        public int Count { get; init; }
        public string? Period { get; init; }
        public string Planet { get; init; }
    }
}