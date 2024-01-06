using System.Text;
using System.Text.Json;
using AnnotatedResult;
using Serilog;
using CleanArch.Shared.Contracts;
using CleanArch.Shared.Contracts.Identity;

namespace CleanArch.WebApp.Services;

public sealed class IdentityClientService
{
    private readonly HttpClient _httpClient;

    public IdentityClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<SignInResponse>> SignInAsync(string username, string password)
    {
        var request = new SignInRequest(username, password);
        var bodyString = JsonSerializer.Serialize(request);
        var content = new StringContent(bodyString, Encoding.UTF8, "application/json");


        using var responseMessage = await _httpClient.PostAsync("api/sign-in", content);

        string responseContent;
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        if (!responseMessage.IsSuccessStatusCode)
        {
            responseContent = await responseMessage.Content.ReadAsStringAsync();
            var statusCode = responseMessage.StatusCode;
            var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(responseContent, options);

            Log.Fatal("Status Code {0}", responseMessage.StatusCode);
            Log.Fatal("Content {0}", responseContent);
            Log.Fatal("Problem Details {0}", problemDetails);

            return Result<SignInResponse>.Error((int)statusCode);
        }

        responseContent = await responseMessage.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<SignInResponse>(responseContent, options);
        Log.Fatal("Content {0}", responseContent);
        Log.Fatal("Access Token {0}", response!.AccessToken);

        return Result<SignInResponse>.Ok(response!);
    }
}
