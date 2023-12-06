namespace CleanArch.Infrastructure.Security;

internal class UserSecrets
{
    internal static class Element
    {
        public const string ApiKey = "UserSecrets:ApiKey";
    }

    public const string SectionName = "UserSecrets";

    public string ApiKey { get; set; } = "";
}
