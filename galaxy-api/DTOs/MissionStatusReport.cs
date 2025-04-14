namespace galaxy_api.DTOs
{
    public readonly struct MissionStatusReport
    {
        public string MissionType { get; init; }
        public string Status { get; init; }
        public int Count { get; init; }
        public string? Period { get; init; }
        public string Planet { get; init; }  

        public MissionStatusReport(string missionType, string status, int count, string? period = null, string planet = "")
        {
            MissionType = missionType;
            Status = status;
            Count = count;
            Period = period;
            Planet = planet;
        }
    }
}
