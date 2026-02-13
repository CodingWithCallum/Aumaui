using System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Core
{
    public record UserIdentity
    {
        [Required]
        public string ExternalId { get; init; } = string.Empty;

        [Required]
        public string MicrosoftId { get; init; } = string.Empty;

        [Required]
        public string UserName { get; init; } = string.Empty;

        // Identity validation
        public bool IsValid => !string.IsNullOrEmpty(ExternalId) &&
                              !string.IsNullOrEmpty(MicrosoftId) &&
                              !string.IsNullOrEmpty(UserName);

        // Display helpers
        public string MaskedExternalId => MaskId(ExternalId);
        public string MaskedMicrosoftId => MaskId(MicrosoftId);

        private static string MaskId(string id) =>
            id.Length > 8 ? $"{id[..4]}****{id[^4..]}" : "****";
    }
}
