using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record SignUpResource(
    [Required(ErrorMessage = "iam.error.email.required")]
    [EmailAddress(ErrorMessage = "iam.error.email.invalid")]
    [StringLength(100, ErrorMessage = "iam.error.email.tooLong")] string Email,
    [Required(ErrorMessage = "iam.error.password.required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "iam.error.password.tooShort")] string Password,
    string? UserType = null);
