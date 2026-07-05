using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

/// <summary>Payload for passwordless client sign-in — just the client's username (email).</summary>
public record ClientSignInResource(
    [Required(ErrorMessage = "iam.error.email.required")] string Username);
