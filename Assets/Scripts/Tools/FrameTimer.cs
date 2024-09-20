using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class FrameTimer : Timer
    {
        public FrameTimer(int time)
        {
            MaxFrame = time;
            LastFrame = time;
            Frame = time;
            FrameModular = 0;
        }
        public override void Run(float speed)
        {
            LastFrame = Frame;
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
        public bool PassedInterval(int interval)
        {
            var lastPassed = MaxFrame - LastFrame;
            var passed = MaxFrame - Frame;
            return lastPassed / interval != passed / interval;
        }
        public void Stop()
        {
            LastFrame = 0;
            Frame = 0;
            FrameModular = 0;
        }
        public void Reset()
        {
            LastFrame = MaxFrame;
            Frame = MaxFrame;
            FrameModular = 0;
        }
        public override bool Expired => Frame <= 0;
        [BsonElement("maxFrame")]
        public int MaxFrame { get; set; }
        [BsonElement("lastFrame")]
        public int LastFrame { get; set; }
        [BsonElement("frame")]
        public int Frame { get; set; }
        [BsonElement("frameModular")]
        public int FrameModular { get; set; }
    }
}