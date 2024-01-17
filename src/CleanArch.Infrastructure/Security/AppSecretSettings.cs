namespace CleanArch.Infrastructure.Security;

public class AppSecretSettings
{
    public static class Element
    {
        public const string MasterKey = "AppSecrets:MasterKey";
        public const string ConnectionString = "AppSecrets:ConnectionString";
    }

    public const string SectionName = "AppSecrets";

    public string MasterKey { get; set; } = "";
    public string ConnectionString { get; set; } = "";
}
