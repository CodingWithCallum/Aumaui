namespace AumauiCL.Models.User.Core
{
    public class UserAccountStatus
    {
        public bool CanSignIn { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsTerminated { get; set; }
    }
}