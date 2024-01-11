#nullable disable
using CleanArch.Shared.Interfaces.Models.Identity;

namespace CleanArch.Shared.Contracts.Identity.Authentication;

public record SignInRequest(string Username, string Password) : IAuthenticationModel;
