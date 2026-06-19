using System.ComponentModel.DataAnnotations;

namespace atelier_platform_aplicaciones_web.IAM.Interfaces.REST.Resources;

public record GoogleSignInResource([Required(ErrorMessage = "iam.error.idToken.required")] string IdToken);
