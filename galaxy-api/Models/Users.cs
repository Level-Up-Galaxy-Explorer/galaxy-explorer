namespace galaxy_api.Models
{
    public class Users{
        public int User_Id {get;set;}
        public string Full_Name {get;set;} = string.Empty;
        public string Email_Address {get;set;} = string.Empty;
        public string Google_Id {get;set;} = string.Empty;
        public int Rank_Id {get;set;}
        public bool? Is_Active {get;set;}
        public DateTime? Created_At {get;set;}  
    }
}