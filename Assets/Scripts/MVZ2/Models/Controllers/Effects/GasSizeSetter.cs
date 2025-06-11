﻿using UnityEngine;

namespace MVZ2.Models
{
    public class GasSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);
            var yHeight = size.y + size.z * 0.5f;
            var volume = size.x * yHeight;

            var gasPosition = new Vector3(0, size.y * 0.5f, 1);
            var gasScale = new Vector3(size.x, yHeight, 1);

            bool stopped = Model.GetProperty<bool>(PROP_STOPPED);
            if (gas)
            {
                var gasPS = gas.Particles;
                var gasEmission = gasPS.emission;
                var gasShape = gasPS.shape;
                gas.OverrideRateOverTime(volume * ratePerVolume);
                gasShape.position = gasPosition;
                gasShape.scale = gasScale;
                if (stopped)
                {
                    if (gasPS.isEmitting)
                        gasPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!gasPS.isEmitting)
                        gasPS.Play(true);
                }
            }

            if (gasLight)
            {
                var gasLightPS = gasLight.Particles;
                var gasLightEmission = gasLightPS.emission;
                var gasLightShape = gasLightPS.shape;
                gasLight.OverrideRateOverTime(volume * ratePerVolume);

                gasLightShape.position = gasPosition;
                gasLightShape.scale = gasScale;

                if (stopped)
                {
                    if (gasLightPS.isEmitting)
                        gasLightPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!gasLightPS.isEmitting)
                        gasLightPS.Play(true);
                }

            }

            if (smoke)
            {
                var smokePS = smoke.Particles;
                var smokeShape = smokePS.shape;
                smokeShape.scale = new Vector3(size.x, 1, 0.1f);
                if (stopped)
                {
                    if (smokePS.isEmitting)
                        smokePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!smokePS.isEmitting)
                        smokePS.Play(true);
                }
            }
        }
        [SerializeField]
        private ParticlePlayer gasLight;
        [SerializeField]
        private ParticlePlayer gas;
        [SerializeField]
        private ParticlePlayer smoke;
        [SerializeField]
        private float ratePerVolume = 20;
        public const string PROP_STOPPED = "Stopped";
    }
}
