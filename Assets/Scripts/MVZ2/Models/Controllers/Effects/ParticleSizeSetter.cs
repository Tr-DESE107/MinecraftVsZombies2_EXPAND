﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ParticleSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);

            var shape = particles.Particles.shape;
            shape.scale = size;
            shape.position = Vector2.up * size.y * 0.5f;
        }
        [SerializeField]
        private ParticlePlayer particles;
    }
}
