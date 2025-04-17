using System.Text.Json.Serialization;

namespace galaxy_api.DTOs;

public class MissionDTO
{
    public int Mission_Id {get;set;}
    public string Name {get;set;} = string.Empty;
    public string Mission_Type {get;set;} = string.Empty;
    public DateTime Launch_Date {get;set;}  
    public string Planet_Type {get;set;} = string.Empty;
    public string Status_Type {get;set;} = string.Empty;
    public string Reward_Credit {get;set;} = string.Empty;
    public string Feedback {get;set;} = string.Empty;
    public string Created_By_Name { get; set; } = string.Empty;  
}
