﻿using UnityEngine;

namespace MVZ2.Models
{
    public class GasBurnSizeSetter : ModelComponent
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

            var timeout = Model.GetProperty<int>("Timeout");
            var percentage = Mathf.Clamp01((maxTime - timeout) / (float)expandTime);

            var firePS = fires.Particles;
            var fireShape = firePS.shape;
            fires.OverrideRateOverTime(volume * ratePerVolume * percentage);
            fireShape.position = gasPosition;
            fireShape.scale = gasScale * percentage;
        }
        [SerializeField]
        private ParticlePlayer fires;
        [SerializeField]
        private int expandTime = 15;
        [SerializeField]
        private int maxTime = 45;
        [SerializeField]
        private float ratePerVolume = 200;
        public const string PROP_STOPPED = "Stopped";
    }
}
