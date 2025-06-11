﻿using UnityEngine;

namespace MVZ2.Models
{
    public class SlendermanTentacle : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            var rng = Model.GetRNG();
            tentacleAnimator.SetFloat("Speed", rng.Next(0.8f, 1.25f));
        }
        [SerializeField]
        private Animator tentacleAnimator;
    }
}
