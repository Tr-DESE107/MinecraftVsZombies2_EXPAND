namespace Tools
{
    public abstract class Timer
    {
        public void Run()
        {
            Run(1);
        }
        public abstract void Run(float speed);
        public abstract void Stop();
        public abstract void Reset();
        public float GetPassedPercentage()
        {
            return 1 - GetTimeoutPercentage();
        }
        public abstract float GetTimeoutPercentage();
        public abstract bool Expired { get; }
    }
}