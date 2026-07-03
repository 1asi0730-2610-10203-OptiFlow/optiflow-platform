using System.ComponentModel.DataAnnotations;

namespace optiflow_platform.IAM.Interfaces.REST.Resources;

public record GoogleSignInResource([Required(ErrorMessage = "iam.error.idToken.required")] string IdToken);
