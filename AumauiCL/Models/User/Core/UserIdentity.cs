using System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Core
{
    public record UserIdentity
    {
        [Required]
        public string ExternalId { get; init; } = string.Empty;

        public string? MicrosoftId { get; init; }

        [Required]
        public string UserName { get; init; } = string.Empty;

        // Identity validation
        public bool IsValid => !string.IsNullOrEmpty(ExternalId) &&
                              !string.IsNullOrEmpty(UserName);

        // Display helpers
        public string MaskedExternalId => MaskId(ExternalId);
        public string MaskedMicrosoftId => MicrosoftId is not null ? MaskId(MicrosoftId) : "N/A";

        private static string MaskId(string id) =>
            id.Length > 8 ? $"{id[..4]}****{id[^4..]}" : "****";
    }
}
