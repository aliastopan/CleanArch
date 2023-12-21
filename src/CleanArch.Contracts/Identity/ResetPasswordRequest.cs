namespace CleanArch.Contracts.Identity;

public record ResetPasswordRequest(Guid UserAccountId,
    string OldPassword,
    string NewPassword,
    string ConfirmPassword);
