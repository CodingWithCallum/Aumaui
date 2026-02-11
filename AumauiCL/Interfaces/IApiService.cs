using MyMauiApp.Models;

namespace AumauiCL.Interfaces;

public interface IApiService
{
    Task<List<BusinessItem>> GetBusinessItemsAsync();
}
