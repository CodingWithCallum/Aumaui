using AumauiCL.Models.Api;

namespace AumauiCL.Interfaces;

public interface IApiService
{
    // Generic data operations
    Task<List<T>> GetItemsAsync<T>(string endpoint);
    Task<T> PushItemAsync<T>(string endpoint, T item);

    // Generic HTTP helper
    Task<TRes> PostAsync<TReq, TRes>(string endpoint, TReq body);

    // Authentication endpoints
    Task<APIResponse<LoginResponse>> XsysLoginAsync(APIRequest<LoginRequest> request);
    Task<APIResponse<LoginResponse>> MicrosoftLoginAsync(APIRequest<MicrosoftLoginRequest> request);
    Task<APIResponse<LoginResponse>> RefreshTokenAsync(APIRequest request);
}
