using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.ResetPassword;

public record ResetPasswordRequest(Guid UserAccountId, string OldPassword, string NewPassword, string ConfirmPassword)
    : IResetPasswordModel;