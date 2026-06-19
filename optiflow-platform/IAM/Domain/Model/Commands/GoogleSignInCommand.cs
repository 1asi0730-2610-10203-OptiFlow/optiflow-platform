using System;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Commands;

public record GoogleSignInCommand
{
    public GoogleSignInCommand(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new ArgumentException("iam.error.idToken.required");
        }
        IdToken = idToken;
    }

    public string IdToken { get; init; }
}
