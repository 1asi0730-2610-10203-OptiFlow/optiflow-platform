namespace atelier_platform_aplicaciones_web.IAM.Domain.Model;

public enum IamError
{
    None,
    UserNotFound,
    EmailAlreadyInUse,
    InvalidCredentials,
    InvalidGoogleToken,
    InvalidCurrentPassword,
    InvalidOrExpiredToken,
    ExpiredOrUsedToken,
    DatabaseError,
    InternalServerError
}
