namespace AMPAI.Core.Models;

public class SessionMetadata
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string AmpModel { get; set; } // e.g., "Katana MkII"
    public int SampleRate { get; set; } = 44100;
    public string FilePath { get; set; } // Path to saved session file
}
