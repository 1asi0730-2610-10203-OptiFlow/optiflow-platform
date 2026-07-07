using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record PasswordRecoveryResource(
    [Required(ErrorMessage = "iam.error.email.required")]
    [EmailAddress(ErrorMessage = "iam.error.email.invalid")]
    [StringLength(100, ErrorMessage = "iam.error.email.tooLong")] string Email);
