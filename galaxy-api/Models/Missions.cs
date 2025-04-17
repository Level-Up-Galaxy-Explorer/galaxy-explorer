namespace galaxy_api.Models
{
    public class Missions
    {
        public int Mission_Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Mission_Type_Id { get; set; }
        public string Mission_Type { get; set; } = string.Empty;
        public DateTime Launch_Date { get; set; }
        public int Destination_Planet_Id { get; set; }
        public string Planet_Type { get; set; } = string.Empty;
        public int Status_Id { get; set; }
        public string Status_Type { get; set; } = string.Empty;
        public string Reward_Credit { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public int Created_By { get; set; }
        public string Created_By_Name { get; set; } = string.Empty;
    }
}