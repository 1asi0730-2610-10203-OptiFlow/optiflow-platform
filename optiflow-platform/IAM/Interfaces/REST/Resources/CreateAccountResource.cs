using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record CreateAccountResource(
    [Required(ErrorMessage = "iam.error.account.name.required")] string Name);
