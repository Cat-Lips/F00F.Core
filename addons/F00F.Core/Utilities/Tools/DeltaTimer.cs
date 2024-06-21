namespace F00F;

public partial class DeltaTimer
{
    public float Time { get; set; } = 1;
    private float time;

    public bool Ready(float delta)
    {
        time += delta;

        if (time >= Time)
        {
            time -= Time;
            return true;
        }

        return false;
    }

    public void Reset()
        => time = 0;
}
