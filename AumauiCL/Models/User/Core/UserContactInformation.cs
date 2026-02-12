using SQLite;

namespace AumauiCL.Models.User.Core
{
   public class UserContactInformation
   {
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        //Look at a way to make mobile number and telephone share the same property
        //Could work as a phone list?
   } 
}