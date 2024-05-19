using UnityEngine;

namespace PVZEngine
{
    public class FrameTimer : Timer
    {
        public FrameTimer(int time)
        {
            MaxFrame = time;
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
        }
        public void Stop()
        {
            Frame = 0;
            FrameModular = 0;
        }
        public void Reset()
        {
            Frame = MaxFrame;
            FrameModular = 0;
        }
        public override bool Expired => Frame <= 0;
        public int MaxFrame { get; set; }
        public int Frame { get; set; }
        public int FrameModular { get; set; }
    }
}