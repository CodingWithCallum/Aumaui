using SQLite;

namespace AumauiCL.Models.User.Core
{
    public class UserAccountStatus
    {
        public bool CanSignIn { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsTerminated { get; set; }

        // Computed property for UI logic
        [Ignore]
        public bool IsAccountActive => CanSignIn && !IsLockedOut && !IsTerminated;

        // Status summary for display
        [Ignore]
        public string StatusSummary => IsAccountActive ? "Active" :
                                     IsTerminated ? "Terminated" :
                                     IsLockedOut ? "Locked" : "Inactive";
    }
}
