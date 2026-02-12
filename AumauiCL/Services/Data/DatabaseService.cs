using AumauiCL.Interfaces;
using SQLite;

namespace AumauiCL.Services.Data
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            var dbName = "AumauiDB.db3";
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);
        }

        private async Task InitAsync()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(_dbPath);

            // Create tables for all known types
            // Note: In a real app, we might use reflection or a manual list
            await _database.CreateTableAsync<Models.User.UserModel>();
            await _database.CreateTableAsync<Models.Audits.AuditAggregate>();
            await _database.CreateTableAsync<Models.Checklists.ChecklistAggregate>();
            await _database.CreateTableAsync<Models.Incidents.IncidentAggregate>();
            await _database.CreateTableAsync<Models.JobCards.JobCardAggregate>();
            await _database.CreateTableAsync<Models.RiskAssessments.RiskAssessmentAggregate>();
            await _database.CreateTableAsync<Models.Attachments.Attachment>();
        }

        public async Task<List<T>> GetItemsAsync<T>() where T : new()
        {
            await InitAsync();
            return await _database!.Table<T>().ToListAsync();
        }

        public async Task<T> GetItemAsync<T>(int id) where T : new()
        {
            await InitAsync();
            return await _database!.FindWithQueryAsync<T>($"SELECT * FROM {typeof(T).Name} WHERE ID = ?", id);
        }

        public async Task<int> SaveItemAsync<T>(T item) where T : new()
        {
            await InitAsync();
            var entity = item as IEntity;
            if (entity == null) throw new InvalidOperationException("Item must implement IEntity");

            if (entity.ID != 0)
            {
                return await _database!.UpdateAsync(item);
            }
            else
            {
                return await _database!.InsertAsync(item);
            }
        }

        public async Task<int> DeleteItemAsync<T>(T item) where T : new()
        {
            await InitAsync();
            return await _database!.DeleteAsync(item);
        }

        // Specific query for getting unsynced items
        public async Task<List<T>> GetUnsyncedItemsAsync<T>() where T : new()
        {
            await InitAsync();
            // This requires the generic T to have a visible SyncState property in the query
            // SQLite-net-pcl isn't great at deep property queries on complex objects serialized as JSON
            // For now, we will fetch all and filter in memory, or strictly rely on a flat 'IsSynced' column if we added one.
            // Current design has SyncState as a property.

            var allItems = await _database!.Table<T>().ToListAsync();
            var syncableItems = allItems.OfType<ISyncable>().Where(x => !x.SyncState.IsSynced).Cast<T>().ToList();
            return syncableItems;
        }
    }
}