using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record ResetPasswordResource(
    [Required(ErrorMessage = "iam.error.token.required")] string Token,
    [Required(ErrorMessage = "iam.error.password.required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "iam.error.password.tooShort")] string NewPassword);
