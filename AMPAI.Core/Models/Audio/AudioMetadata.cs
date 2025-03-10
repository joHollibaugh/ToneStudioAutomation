namespace AMPAI.Core.Models;

public class AudioMetadata
{
    public string FilePath { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public int SampleRate { get; set; }
    public int BitDepth { get; set; }
    public int Channels { get; set; }
}