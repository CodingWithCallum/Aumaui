using SQLite;

namespace MyMauiApp.Models;

public class SyncLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Category { get; set; } = "General";
    public DateTime SyncTime { get; set; }
    public bool WasSuccessful { get; set; }
}