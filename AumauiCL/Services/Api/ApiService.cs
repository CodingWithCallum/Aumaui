using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AumauiCL.Interfaces;
using AumauiCL.Models.Api;

namespace AumauiCL.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://localhost:7224/api";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient)
    {
        _http = httpClient;
        _http.BaseAddress ??= new Uri(BaseUrl);
    }

    // ─── Generic HTTP Helper ───────────────────────────────────────────

    public async Task<TRes> PostAsync<TReq, TRes>(string endpoint, TReq body)
    {
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.PostAsync(endpoint, jsonContent);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TRes>(content, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize response from {endpoint}");
    }

    // ─── Authentication Endpoints ──────────────────────────────────────

    public async Task<APIResponse<LoginResponse>> XsysLoginAsync(APIRequest<LoginRequest> request)
    {
        return await PostAsync<APIRequest<LoginRequest>, APIResponse<LoginResponse>>(
            "/Authentication/XsysLogin", request);
    }

    public async Task<APIResponse<LoginResponse>> MicrosoftLoginAsync(APIRequest request)
    {
        return await PostAsync<APIRequest, APIResponse<LoginResponse>>(
            "/Authentication/MicrosoftLogin", request);
    }

    public async Task<APIResponse<LoginResponse>> RefreshTokenAsync(APIRequest request)
    {
        return await PostAsync<APIRequest, APIResponse<LoginResponse>>(
            "/Authentication/XsysRefreshCurrentToken", request);
    }

    // ─── Generic Data Operations ───────────────────────────────────────

    public async Task<List<T>> GetItemsAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<T>>(content, JsonOptions)
            ?? new List<T>();
    }

    public async Task<T> PushItemAsync<T>(string endpoint, T item)
    {
        return await PostAsync<T, T>(endpoint, item);
    }
}
