namespace CleanArch.Api.Endpoints;

public static class ApiEndpoint
{
    public static class Identity
    {
        public const string SignIn = "/api/sign-in";
        public const string SignOut = "/api/sign-out";
        public const string SignUp = "/api/sign-up";
        public const string Refresh = "/api/sign-in/refresh";
        public const string ResetPassword = "/api/identity/reset-password";
        public const string GrantRole = "/api/identity/grant-role";
        public const string RevokeRole = "/api/identity/revoke-role";
        public const string GetUsers = "/api/identity/get-all";
    }
}
