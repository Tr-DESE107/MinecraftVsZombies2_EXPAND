using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void WriteToSerializable_Twinkle(SerializableLevelController seri)
        {
            seri.twinkleTime = twinkleTime;
        }
        private void ReadFromSerializable_Twinkle(SerializableLevelController seri)
        {
            twinkleTime = seri.twinkleTime;
        }
        public float GetTwinkleAlpha()
        {
            var clamped = (Mathf.Cos(twinkleTime * Mathf.PI * 2) + 1) * 0.5f;
            return clamped * 0.75f;
        }
        public Color GetTwinkleColor()
        {
            var c = 1 - GetTwinkleAlpha();
            return new Color(c, c, c, 1);
        }
        private void UpdateTwinkle(float deltaTime)
        {
            var speed = 2;
            twinkleTime += deltaTime * speed;
            twinkleTime %= 1;
        }

        #region 属性字段
        private float twinkleTime;
        #endregion
    }
}
