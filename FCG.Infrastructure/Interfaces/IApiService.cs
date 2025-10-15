namespace FCG.Infrastructure.Interfaces;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data);
}
