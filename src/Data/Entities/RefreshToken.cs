using Data.Common.Entity;

namespace Data.Entities;

public class RefreshToken : ReadOnlyEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Token { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsRevoked() => RevokedAtUtc.HasValue;
    public bool IsActive() => RevokedAtUtc == null && DateTime.UtcNow < ExpiresAtUtc;
}