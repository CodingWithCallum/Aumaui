using AumauiCL.Interfaces;
using SQLite;

namespace AumauiCL.Services.Data
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public DatabaseService()
        {
            var dbName = "AumauiDB.db3";
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, dbName);
        }

        public async Task InitAsync()
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
            _ = await _database.CreateTableAsync<Models.User.Core.UserRole>();
            _ = await _database.CreateTableAsync<Models.User.Extended.UserAudit>();
            _ = await _database.CreateTableAsync<Models.User.Extended.UserPreferences>();

            // Legacy / Other - Removed
        }

        public async Task<List<T>> GetItemsAsync<T>() where T : class, IEntity, new()
        {
            await InitAsync();
            return await _database!.Table<T>().ToListAsync();
        }

        public async Task<T?> GetItemAsync<T>(int id) where T : class, IEntity, new()
        {
            await InitAsync();
            return await _database!.FindWithQueryAsync<T>($"SELECT * FROM {typeof(T).Name} WHERE ID = ?", id);
        }

        public async Task<int> SaveItemAsync<T>(T item) where T : class, IEntity, new()
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

        public async Task<int> DeleteItemAsync<T>(T item) where T : class, IEntity, new()
        {
            await InitAsync();
            return await _database!.DeleteAsync(item);
        }

        // Specific query for getting unsynced items
        public async Task<List<T>> GetUnsyncedItemsAsync<T>() where T : class, IEntity, ISyncable, new()
        {
            await InitAsync();
            // This requires the generic T to have visible IsSynced properties in the query
            // SQLite-net-pcl isn't great at deep property queries on complex objects serialized as JSON
            // We now have flat 'IsSynced' column.

            var allItems = await _database!.Table<T>().ToListAsync();
            var syncableItems = allItems.OfType<ISyncable>().Where(x => !x.IsSynced).Cast<T>().ToList();
            return syncableItems;
        }
    }
}