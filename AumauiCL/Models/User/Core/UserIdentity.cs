using SQLite;

namespace AumauiCL.Models.User.Core
{
   public class UserIdentity
   {
        [PrimaryKey, AutoIncrement]
        public int ID {get;set;}

        [Unique]
        public string ExternalId { get; set; } = string.Empty;

        [Unique]
        public string MicrosoftId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
   } 
}