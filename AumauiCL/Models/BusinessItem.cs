using SQLite;

namespace MyMauiApp.Models;

public class BusinessItem
{
    [PrimaryKey]
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
