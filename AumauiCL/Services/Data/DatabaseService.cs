using MyMauiApp.Models;
using SQLite;


namespace MyMauiApp.Services.Data;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    // Define the database path
    private static string DatabasePath =>
        Path.Combine(FileSystem.AppDataDirectory, "AppDatabase.db3");

    // Initialize the connection and create tables
    private async Task Init()
    {
        if (_database is not null)
            return;

        // Flags: Create if doesn't exist, Read/Write, and Multi-threaded access
        var flags = SQLiteOpenFlags.ReadWrite |
                    SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.SharedCache;

        _database = new SQLiteAsyncConnection(DatabasePath, flags);

        // Define your tables here. Add more as your app grows.
        await _database.CreateTableAsync<UserRecord>();
        await _database.CreateTableAsync<SyncLog>();
        await _database.CreateTableAsync<BusinessItem>();
    }

    // Generic "Upsert" - Perfect for Syncing
    public async Task UpsertItemAsync<T>(T item) where T : new()
    {
        await Init();
        await _database!.InsertOrReplaceAsync(item);
    }

    // Generic Fetch All
    public async Task<List<T>> GetItemsAsync<T>() where T : new()
    {
        await Init();
        return await _database!.Table<T>().ToListAsync();
    }

    // Specific query example: Get the latest sync time
    public async Task<DateTime?> GetLastSyncTimeAsync(string category)
    {
        await Init();
        var log = await _database!.Table<SyncLog>()
                                  .Where(x => x.Category == category)
                                  .OrderByDescending(x => x.SyncTime)
                                  .FirstOrDefaultAsync();
        return log?.SyncTime;
    }

    // Clear data on Logout
    public async Task PurgeSensitiveDataAsync()
    {
        await Init();
        await _database!.DeleteAllAsync<UserRecord>();
        // We keep the SyncLog so we know when we last fetched data
    }
}