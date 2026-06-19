using System.ComponentModel.DataAnnotations;

namespace APIsNABH.dto
{
    public class DtoCreateUserRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Phone]
        [StringLength(15)]
        public string? Phone { get; set; }

        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid Mobile Number")]
        public string? Mobile { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string OrganizationName { get; set; }

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN Number")]
        public string? PanNumber { get; set; }

        public string? TanNumber { get; set; }

        public bool IsGstAvailable { get; set; }

        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{3}$", ErrorMessage = "Invalid GST Number")]
        public string? GstNumber { get; set; }

    }
}
