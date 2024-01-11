using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.Authentication;

public record RefreshAccessRequest(string AccessToken, string RefreshTokenStr) : IRefreshAccessModel;
