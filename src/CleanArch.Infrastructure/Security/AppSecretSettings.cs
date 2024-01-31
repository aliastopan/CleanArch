namespace CleanArch.Infrastructure.Security;

public class AppSecretSettings
{
    public static class Section
    {
        public const string MasterKey = "AppSecrets:MasterKey";
        public const string MySqlConnectionString = "AppSecrets:MySqlConnectionString";
    }

    public const string SectionName = "AppSecrets";

    public string MasterKey { get; set; } = "";
    public string MySqlConnectionString { get; set; } = "";
}
