namespace CleanArch.Infrastructure.Security;

internal sealed class SecurityTokenSettings
{
    internal static class Element
    {
        public const string Issuer = "SecurityToken:Issuer";
        public const string Audience = "SecurityToken:Audience";
    }

    public const string SectionName = "SecurityToken";

    public TimeSpan AccessTokenLifeTime { get; init; }
    public TimeSpan RefreshTokenLifeTime { get; init; }
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
}
