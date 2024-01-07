namespace CleanArch.Infrastructure.Security;

public class InfrastructureSecretSettings
{
    public static class Element
    {
        public const string MasterKey = "InfrastructureSecrets:MasterKey";
    }

    public const string SectionName = "InfrastructureSecrets";

    public string MasterKey { get; set; } = "";
}
