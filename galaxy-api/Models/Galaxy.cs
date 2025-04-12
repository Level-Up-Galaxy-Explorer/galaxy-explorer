namespace galaxy_api.Models
{
    public class Galaxy
    {
        public int Galaxy_Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Galaxy_Type_Id { get; set; }
        public int Distance_From_Earth { get; set; }
        public string Description { get; set; } = string.Empty;

    }
}