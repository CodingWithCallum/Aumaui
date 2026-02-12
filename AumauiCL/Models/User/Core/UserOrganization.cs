namespace AumauiCL.Models.User.Core
{
    public class UserOrganization
    {
        public string CompanyCode { get; set; } = string.Empty;
        public int CompanyID { get; set; }
        public string Company { get; set; } = string.Empty;

        public int DivisionID { get; set; }
        public string Division { get; set; } = string.Empty;

        public int DepartmentID { get; set; }
        public string Department { get; set; } = string.Empty;

        public int TeamID { get; set; }
        public string Team { get; set; } = string.Empty;
    }
}