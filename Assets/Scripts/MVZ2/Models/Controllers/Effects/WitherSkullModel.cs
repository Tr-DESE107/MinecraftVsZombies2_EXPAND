﻿using UnityEngine;

namespace MVZ2.Models
{
    public class WitherSkullModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var source = Lawn2TransPosition(Model.GetProperty<Vector3>("Source"));
            var dest = Lawn2TransPosition(Model.GetProperty<Vector3>("Dest"));
            var distance = dest - source;
            distance.y -= distance.z;
            sourceTransform.localEulerAngles = Vector3.zero;

            var velocityHori = new Vector2(distance.x, distance.z);
            var horiAngle = Vector2.SignedAngle(velocityHori, Vector2.right);
            sourceTransform.Rotate(Vector3.up, horiAngle);

            var vertAngle = Vector2.SignedAngle(new Vector2(velocityHori.magnitude, distance.y), Vector2.right);
            sourceTransform.Rotate(Vector3.back, vertAngle);
        }
        [SerializeField]
        private Transform sourceTransform;
    }
}
