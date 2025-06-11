﻿namespace MVZ2.Models
{
    public class FireTimeRunner : ShaderPropertySetter<float>
    {
        public override void UpdateFrame(float deltaTime)
        {
            fireTime += deltaTime;
            base.UpdateFrame(deltaTime);
        }
        public override void SetProperty(float value)
        {
            Element.SetFloat("_FireTime", value);
        }
        public override float GetDefaultValue() => 0;
        public override float GetCurrentValue() => fireTime;
        private float fireTime = 0;

    }
}
