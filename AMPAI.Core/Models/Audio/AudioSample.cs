namespace AMPAI.Core.Models.Audio;

public class AudioSample
{
    public DateTime Timestamp { get; set; }
    public byte[] Data { get; set; } // Raw PCM or WAV data
    public AudioMetadata meta { get; set; }
}
