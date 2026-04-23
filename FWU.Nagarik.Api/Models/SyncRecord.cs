namespace FWU.Nagarik.Api.Models;

public class SyncRecord
{
    public int Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public DateTime LastSyncTime { get; set; }
    public int TotalRecordsSynced { get; set; }
    public string LoadedBy { get; set; } = string.Empty;
}
