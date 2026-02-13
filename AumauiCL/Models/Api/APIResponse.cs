namespace AumauiCL.Models.Api
{
    public class APIResponse
    {
        public string? ResponseMessage { get; set; }
        public List<APIResponseValidation> ResponseValidation { get; set; } = new();
        public bool IsSuccessful { get; set; } = true;
        public int? ResponseID { get; set; }
        public string? ResponseStringID { get; set; }
    }

    public class APIResponse<T> : APIResponse
    {
        public T ResponseData { get; set; } = default!;   // Strongly-typed response data
    }

    public class APIResponseValidation
    {
        /// <summary>
        /// Position of the validation issue
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Description of the validation error
        /// </summary>
        public string? ValidationMessage { get; set; }

        /// <summary>
        /// This is the value of the property that failed validation
        /// </summary>
        public string? Value { get; set; }
    }
}
