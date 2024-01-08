using System.Text;
using System.Text.Json;
using CleanArch.Shared.Contracts.Identity;

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
        var bodyString = JsonSerializer.Serialize(request);
        var content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        using var responseMessage = await _httpClient.PostAsync("api/sign-in", content);

        return new HttpResult
        {
            IsSuccessStatusCode = responseMessage.IsSuccessStatusCode,
            Headers = responseMessage.Headers,
            Content = await responseMessage.Content.ReadAsStringAsync()
        };
    }
}
