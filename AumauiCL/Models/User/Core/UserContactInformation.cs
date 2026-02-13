using SQLite;
using Annotation = System.ComponentModel.DataAnnotations;

namespace AumauiCL.Models.User.Core
{
    public record UserContactInformation
    {
        [Annotation.Required, Annotation.EmailAddress]
        public string Email { get; init; } = string.Empty;

        // Single phone list instead of separate properties
        [Ignore]
        public List<PhoneNumber> PhoneNumbers { get; init; } = new();

        // For backward compatibility and SQLite storage
        [Obsolete("Use PhoneNumbers collection instead")]
        public string Telephone { get; init; } = string.Empty;

        [Obsolete("Use PhoneNumbers collection instead")]
        public string MobileNumber { get; init; } = string.Empty;

        // ADD: Helper methods
        [Ignore]
        public string PrimaryPhone => PhoneNumbers.FirstOrDefault(p => p.IsPrimary)?.Number ??
                                     PhoneNumbers.FirstOrDefault()?.Number ??
                                     string.Empty;
    }

    // Phone number value object
    public record PhoneNumber(string Number, PhoneType Type, bool IsPrimary = false)
    {
        public string FormattedNumber => FormatPhoneNumber(Number);

        private static string FormatPhoneNumber(string number) =>
            // Add your phone formatting logic here
            number.Length == 10 ? $"({number[..3]}) {number[3..6]}-{number[6..]}" : number;
    }

    public enum PhoneType { Mobile, Work, Home, Fax }
}
