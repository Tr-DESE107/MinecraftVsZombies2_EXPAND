﻿using UnityEngine;

namespace MVZ2.Models
{
    public class NightmareWatchingEyeModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var direction = Model.GetProperty<Vector2>("EyeDirection");
            pupilTransform.localPosition = direction * moveDistance;
        }
        [SerializeField]
        private Transform pupilTransform;
        [SerializeField]
        private float moveDistance = 1;
    }
}
