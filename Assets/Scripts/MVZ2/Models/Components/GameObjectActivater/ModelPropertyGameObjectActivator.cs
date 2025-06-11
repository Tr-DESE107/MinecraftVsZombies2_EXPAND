﻿namespace MVZ2.Models
{
    public abstract class ModelPropertyGameObjectActivator : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            var active = GetActive();
            if (active != gameObject.activeSelf)
            {
                gameObject.SetActive(active);
            }
        }
        public abstract bool GetActive();
    }
}