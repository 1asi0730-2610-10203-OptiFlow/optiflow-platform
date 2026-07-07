using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record SignInResource(
    [Required(ErrorMessage = "iam.error.email.required")] string Email,
    [Required(ErrorMessage = "iam.error.password.required")] string Password);
