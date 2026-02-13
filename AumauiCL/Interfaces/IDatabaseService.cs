
using AumauiCL.Interfaces;

namespace AumauiCL.Interfaces
{
    public interface IDatabaseService
    {
        Task InitAsync(); // Ensure DB is initialized

        Task<List<T>> GetItemsAsync<T>() where T : class, IEntity, new();
        Task<T?> GetItemAsync<T>(int id) where T : class, IEntity, new();
        Task<int> SaveItemAsync<T>(T item) where T : class, IEntity, new();
        Task<int> DeleteItemAsync<T>(T item) where T : class, IEntity, new();

        Task<List<T>> GetUnsyncedItemsAsync<T>() where T : class, IEntity, ISyncable, new();

        // Add GetUserByEmailAsync if needed, or keep generic GetItemsAsync
    }
}
