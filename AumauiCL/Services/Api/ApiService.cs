using AumauiCL.Interfaces;
using MyMauiApp.Models;

namespace MyMauiApp.Services.Api;

public class ApiService : IApiService
{
    public async Task<List<BusinessItem>> GetBusinessItemsAsync()
    {
        // Simulate network delay
        await Task.Delay(2000);

        return new List<BusinessItem>
        {
            new BusinessItem { Id = "1", Name = "Project Alpha", Description = "Top secret project", LastUpdated = DateTime.Now },
            new BusinessItem { Id = "2", Name = "Project Beta", Description = "Public release candidate", LastUpdated = DateTime.Now.AddDays(-1) },
            new BusinessItem { Id = "3", Name = "Project Gamma", Description = "Internal tools", LastUpdated = DateTime.Now.AddDays(-2) }
        };
    }
}
