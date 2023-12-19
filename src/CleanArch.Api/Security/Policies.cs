namespace CleanArch.Api.Security;

public static class Policies
{
    public const string AllowAnonymous = "allow_anonymous_policy";
    public const string VerifiedUserPolicy = "verified_user_policy";
    public const string AdministratorPrivilege = "administrator_privilege_policy";
}
