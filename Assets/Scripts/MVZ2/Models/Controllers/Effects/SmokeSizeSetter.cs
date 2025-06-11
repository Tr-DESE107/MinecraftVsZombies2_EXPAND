﻿using UnityEngine;

namespace MVZ2.Models
{
    public class SmokeSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Model.GetProperty<bool>(PROP_EMITTED))
                return;
            Model.SetProperty(PROP_EMITTED, true);
            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);
            float volume = size.x * size.y * size.z;

            var ps = particles.Particles;
            var shape = ps.shape;
            var scale = size;
            scale.y *= 0.333333f;
            shape.scale = scale;
            shape.position = Vector2.up * scale.y * 0.5f;
            particles.Emit(100 * volume);
        }
        [SerializeField]
        private ParticlePlayer particles;
        public const string PROP_EMITTED = "Emitted";
    }
}
