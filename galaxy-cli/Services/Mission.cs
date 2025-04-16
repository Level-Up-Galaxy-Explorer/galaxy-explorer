using System.Text.Json.Serialization;

namespace galaxy_api.DTOs;

public class MissionDTO
{
    public int Mission_Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public int Mission_Type_Id {get;set;}
    public DateTime Launch_Date {get;set;}  
    public int Destination_Planet_Id {get;set;}
    public int Status_Id {get;set;}
    public string Reward_Credit {get;set;} = string.Empty;
    public string Feedback {get;set;} = string.Empty;
    public int Created_By { get; set; }
}
