﻿using UnityEngine;

namespace MVZ2.Models
{
    public class DivineShieldModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var size = Model.GetProperty<Vector3>("Size");
            size.x += 32;
            size.y += 32;
            size = Lawn2TransScale(size);
            size *= 1 / 1.28f;
            shieldRoot.localScale = size;
        }
        [SerializeField]
        private Transform shieldRoot;
    }
}
