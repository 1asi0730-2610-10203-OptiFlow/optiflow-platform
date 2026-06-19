using System.Text.Json.Serialization;

using atelier_platform_aplicaciones_web.Shared.Domain.Model.Entities;
using atelier_platform_aplicaciones_web.IAM.Domain.Model.ValueObjects;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    public User() 
    {
        Status = UserStatus.Active;
        Id = null!;
        Email = null!;
        Password = null!;
    }

    public User(EmailAddress email, Password password) : this()
    {
        Id = new UserId(Guid.NewGuid());
        Email = email;
        Password = password;
    }

    public User(EmailAddress email, Password password, GoogleId googleId) : this(email, password)
    {
        GoogleId = googleId;
    }

    public UserId Id { get; private set; }
    public EmailAddress Email { get; private set; }
    
    [JsonIgnore] 
    public Password Password { get; private set; }
    
    public GoogleId? GoogleId { get; private set; }
    public UserStatus Status { get; private set; }

    public User ChangePassword(Password password)
    {
        Password = password;
        return this;
    }

    public User ChangeEmail(EmailAddress email)
    {
        Email = email;
        return this;
    }

    public User Deactivate()
    {
        Status = UserStatus.Inactive;
        return this;
    }

    public User LinkGoogleAccount(GoogleId googleId)
    {
        GoogleId = googleId;
        return this;
    }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public long Version { get; set; }
}

