namespace AMPAI.Core.Models;

public class SyncEvent
{
    public DateTime Timestamp { get; set; }
    public AmpSettingSnapshot Settings { get; set; }
}