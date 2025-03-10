namespace AMPAI.COre.Behaviors;

public class TimestampedData<T>
{
    public DateTime Timestamp { get; set; }
    public T Data { get; set; }
}