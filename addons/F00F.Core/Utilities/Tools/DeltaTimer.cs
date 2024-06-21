namespace F00F;

public partial class DeltaTimer
{
    public float Time { get; private set; }
    public float Interval { get; set; } = 1;
    public float TimeRemaining => Interval - Time;
    public float PercentRemaining => Time / Interval;

    public bool Ready(float delta)
    {
        Time += delta;

        if (Time >= Interval)
        {
            Time -= Interval;
            return true;
        }

        return false;
    }

    public void Reset()
        => Time = 0;
}
