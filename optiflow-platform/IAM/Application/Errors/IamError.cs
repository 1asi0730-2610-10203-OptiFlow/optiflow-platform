namespace optiflow_platform.IAM.Domain.Model;

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
    InternalServerError,
    AccountAlreadyOnboarded
}
