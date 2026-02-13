using System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.Api
{
    public class APIRequest
    {
        [MaxLength(6)]
        public string? Code { get; set; }              // Company Instance Code
        public string? UserID { get; set; }            // SHEQsys User ID
        public string? ProviderKey { get; set; }       // Microsoft User Object ID
        public string? APIAuthKey { get; set; }        // API Authentication Key
        public string? Header { get; set; }            // Basic Auth Header
    }

    public class APIRequest<T> : APIRequest
    {
        public T BodyData { get; set; } = default!;   // Strongly-typed payload
    }
}
