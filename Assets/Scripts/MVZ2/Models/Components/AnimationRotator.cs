﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class AnimationRotator : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            transform.localEulerAngles = rotation;
        }
        [SerializeField]
        private Vector3 rotation;
    }
}
