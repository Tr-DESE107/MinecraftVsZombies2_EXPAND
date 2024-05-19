using UnityEngine;

namespace PVZEngine
{
    public class FrameTimer : Timer
    {
        public int Frame { get; set; }
        public int FrameModular { get; set; }
        public FrameTimer(int time)
        {
            Frame = time;
            FrameModular = 0;
        }
        public override void Run(float speed)
        {
            if (Expired)
                return;
            int integer = (int)speed;
            float modular = speed - integer;
            Frame -= integer;

            if (modular > 0)
            {
                int neededFrame = Mathf.CeilToInt(1 / modular);
                FrameModular++;
                if (FrameModular >= neededFrame)
                {
                    FrameModular = 0;
                    Frame--;
                }
            }
            if (Frame <= 0)
            {
                Expired = true;
            }
        }
        public void Stop()
        {
            Frame = 0;
            FrameModular = 0;
        }
        public void Reset()
        {
            FrameModular = 0;
        }
    }
}