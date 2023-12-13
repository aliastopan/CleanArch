namespace CleanArch.Contracts.Identity;

public record ResetPasswordRequest(Guid UserId,
    string OldPassword,
    string NewPassword,
    string ConfirmPassword);
