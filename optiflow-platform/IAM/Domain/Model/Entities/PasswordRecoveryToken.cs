using System;

namespace atelier_platform_aplicaciones_web.IAM.Domain.Model.Entities;

public partial class PasswordRecoveryToken
{
    public PasswordRecoveryToken() 
    {
        TokenHash = string.Empty;
    }

    public PasswordRecoveryToken(string tokenHash, Guid userId, long expirationMinutes) : this()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddMinutes(expirationMinutes);
        IsUsed = false;
    }

    public Guid Id { get; private set; }
    public string TokenHash { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    public bool IsValid()
    {
        return !IsUsed && ExpiresAt > DateTime.UtcNow;
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
    }
}
