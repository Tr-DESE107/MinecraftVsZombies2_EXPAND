using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace PVZEngine
{
    public static class TimerHelper
    {
        public static IEnumerable<float> IteratePassedFrames(this FrameTimer timer, float interval)
        {
            float lastFrame = timer.LastFrame + timer.LastFrameFraction / (float)timer.Precision;
            float currentFrame = timer.Frame + timer.FrameFraction / (float)timer.Precision;
            float start = Mathf.CeilToInt(lastFrame / interval);
            float end = Mathf.CeilToInt(currentFrame / interval);
            end = Mathf.Max(0, end);
            for (float t = start; t > end; t--)
            {
                yield return (t - 1) * interval;
            }
        }
        public static void ResetSeconds(this FrameTimer timer, float seconds)
        {
            timer.ResetTime(Ticks.FromSeconds(seconds));
        }
        public static FrameTimer NewSecondTimer(float seconds)
        {
            return new FrameTimer(Ticks.FromSeconds(seconds));
        }
    }
}
