using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Core
{
    public record UserOrganization
    {
        [Annotation.Required, Annotation.MaxLength(10)]
        public string CompanyCode { get; init; } = string.Empty;

        public int CompanyID { get; init; }
        public string Company { get; init; } = string.Empty;

        public int DivisionID { get; init; }
        public string Division { get; init; } = string.Empty;

        public int DepartmentID { get; init; }
        public string Department { get; init; } = string.Empty;

        public int TeamID { get; init; }
        public string Team { get; init; } = string.Empty;

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
