﻿using UnityEngine;

namespace MVZ2.Models
{
    public class KnockbackWaveModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            lastPosition = transform.position;
        }
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            float angle = Vector2.SignedAngle(Vector2.right, transVelocity);
            Model.SetProperty("Angle", angle);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            SetDirection(Model.GetProperty<float>("Angle"));
            transVelocity = transform.position - lastPosition;
            lastPosition = transform.position;
        }

        public void SetDirection(float angle)
        {
            particle.transform.eulerAngles = Vector3.forward * angle;
        }
        [SerializeField]
        private ParticleSystem particle;
        private Vector3 lastPosition;
        private Vector3 transVelocity;
    }
}
