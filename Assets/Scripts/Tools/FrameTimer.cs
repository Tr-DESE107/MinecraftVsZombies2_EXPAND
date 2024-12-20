using System;
using MongoDB.Bson.Serialization.Attributes;
using UnityEngine;

namespace Tools
{
    [Serializable]
    public class FrameTimer : Timer
    {
        public FrameTimer() : this(0)
        {
        }
        public FrameTimer(int time) : this(time, DEFAULT_PRECISION)
        {
            MaxFrame = time;
            Frame = time;
            FrameFraction = 0;
            LastFrame = time;
            LastFrameFraction = FrameFraction;
        }
        public FrameTimer(int time, int precision)
        {
            Precision = precision;
        }
        public override void Run(float speed)
        {
            LastFrame = Frame;
            LastFrameFraction = FrameFraction;
            if (Expired)
                return;
            int integer = (int)speed;
            float modular = speed - integer;
            Frame -= integer;

            if (modular > 0)
            {
                FrameFraction += Mathf.FloorToInt(modular * Precision);
                if (FrameFraction >= Precision)
                {
                    FrameFraction -= Precision;
                    Frame--;
                }
            }
        }
        public bool PassedFrame(int frame)
        {
            return LastFrame > frame && Frame <= frame;
        }
        public bool PassedInterval(int interval)
        {
            return PassedIntervalCount(interval) != 0;
        }
        public int PassedIntervalCount(int interval)
        {
            return Mathf.CeilToInt(LastFrame / interval) - Mathf.CeilToInt(Frame / interval);
        }
        public void Stop()
        {
            Frame = 0;
            FrameFraction = 0;
            LastFrame = 0;
            LastFrameFraction = 0;
        }
        public void Reset()
        {
            Frame = MaxFrame;
            FrameFraction = 0;
            LastFrame = MaxFrame;
            LastFrameFraction = 0;
        }
        public void ResetTime(int time)
        {
            MaxFrame = time;
            Reset();
        }
        public override bool Expired => Frame <= 0;
        [BsonElement("maxFrame")]
        public int MaxFrame { get; set; }
        [BsonElement("lastFrame")]
        public int LastFrame { get; set; }
        [BsonElement("lastFrameFraction")]
        public int LastFrameFraction { get; set; }
        [BsonElement("frame")]
        public int Frame { get; set; }
        [BsonElement("frameFraction")]
        public int FrameFraction { get; set; }
        [BsonElement("precision")]
        public int Precision { get; private set; }

        public const int DEFAULT_PRECISION = 2048;
    }
}