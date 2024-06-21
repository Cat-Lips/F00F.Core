namespace F00F
{
    public partial class DeltaTimer(float seconds = 1)
    {
        public float Time { get; set; } = seconds;
        private float time;

        public bool Ready(float delta)
        {
            time += delta;

            if (time >= Time)
            {
                time = 0;
                return true;
            }

            return false;
        }
    }
}
