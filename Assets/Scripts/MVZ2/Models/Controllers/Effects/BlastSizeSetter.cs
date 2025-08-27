﻿using UnityEngine;

namespace MVZ2.Models
{
    public class BlastSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Model.GetProperty<bool>(PROP_EMITTED))
                return;
            Model.SetProperty(PROP_EMITTED, true);


            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);
            float volume = size.x * size.z;
            float maxRadius = Mathf.Max(size.x, size.z);


            var smokePs = player.Particles;

            var smokeMain = smokePs.main;
            smokeMain.startSpeedMultiplier = maxRadius * speedPerRadius;
            player.Emit(particleCountPerVolume * volume + additionalParticleCount);
        }
        [SerializeField]
        private ParticlePlayer player;
        [SerializeField]
        private float particleCountPerVolume = 5;
        [SerializeField]
        private float additionalParticleCount = 10;
        [SerializeField]
        private float speedPerRadius = 0.5f;
        public const string PROP_EMITTED = "BlastEmitted";
    }
}
