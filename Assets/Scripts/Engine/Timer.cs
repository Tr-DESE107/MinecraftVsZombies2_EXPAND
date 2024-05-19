namespace PVZEngine
{
    public abstract class Timer
    {
        public bool Expired { get; protected set; }
        /// <summary>
        /// 运行帧计时器。
        /// </summary>
        /// <param name="alarm">计时器。</param>
        /// <returns>是否结束。</returns>
        public void Run()
        {
            Run(1);
        }

        public abstract void Run(float speed);
    }
}