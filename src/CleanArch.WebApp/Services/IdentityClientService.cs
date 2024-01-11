using System.Text;
using System.Text.Json;
using CleanArch.Shared.Contracts.Identity.Authentication;

namespace CleanArch.WebApp.Services;

public sealed class IdentityClientService
{
    private readonly HttpClient _httpClient;

    public IdentityClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResult> SignInAsync(string username, string password)
    {
        var request = new SignInRequest(username, password);
        var jsonBody = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var responseMessage = await _httpClient.PostAsync("api/sign-in", content);

        return new HttpResult
        {
            IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
            Headers = responseMessage.Headers,
            Content = await responseMessage.Content.ReadAsStringAsync()
        };
    }

    public async Task<HttpResult> RefreshAccessAsync(string accessToken, string refreshTokenStr)
    {
        var request = new RefreshAccessRequest(accessToken, refreshTokenStr);
        var jsonBody = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        using var responseMessage = await _httpClient.PostAsync("api/sign-in/refresh", content);

        return new HttpResult
        {
            IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
            Headers = responseMessage.Headers,
            Content = await responseMessage.Content.ReadAsStringAsync()
        };
    }
}
