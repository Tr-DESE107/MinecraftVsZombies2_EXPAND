﻿using UnityEngine;

namespace MVZ2.Models
{
    public class MagneticLineModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var source = sourceTransform.position;
            var dest = Lawn2TransPosition(Model.GetProperty<Vector3>("Dest"));
            var distance = dest - source;
            sourceTransform.localRotation = Quaternion.FromToRotation(Vector3.right, distance);
            sourceTransform.localScale = new Vector3(distance.magnitude, 1, 1);
        }
        [SerializeField]
        private Transform sourceTransform;
    }
}
