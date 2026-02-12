using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace Client_MAUI_CL.Models.TestingFolder.User.Core
{
    public class UserOrganization
    {
        [Annotation.Required, Annotation.MaxLength(10)]
        public string CompanyCode { get; set; } = string.Empty;

        public int CompanyID { get; set; }
        public string Company { get; set; } = string.Empty;

        public int DivisionID { get; set; }
        public string Division { get; set; } = string.Empty;

        public int DepartmentID { get; set; }
        public string Department { get; set; } = string.Empty;

        public int TeamID { get; set; }
        public string Team { get; set; } = string.Empty;

        // Hierarchy helpers
        [Ignore]
        public string FullHierarchy => $"{Company} > {Division} > {Department} > {Team}".Replace(" >  > ", " > ").TrimEnd(' ', '>');

        [Ignore]
        public bool IsComplete => CompanyID > 0 && DivisionID > 0 && DepartmentID > 0;

        // Display formatting
        [Ignore]
        public string DisplayName => !string.IsNullOrEmpty(Team) ? Team :
                                    !string.IsNullOrEmpty(Department) ? Department :
                                    !string.IsNullOrEmpty(Division) ? Division : Company;
    }
}
