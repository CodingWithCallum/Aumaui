namespace AumauiCL.Models.Audits
{
    public class AuditStatus
    {
        public string StatusName { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public bool IsTerminal { get; set; }
        public string ColorCode { get; set; } = string.Empty;
    }
}
