﻿using MVZ2.Audios;
using UnityEngine;

namespace MVZ2.Models
{
    public class FrankensteinModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            shellCaseParticle.OnParticleCollisionEvent += OnParticleCollisionCallback;
        }
        public override void OnTrigger(string name)
        {
            base.OnTrigger(name);
            if (name == "GunFire")
            {
                shellCaseParticle.Emit(1);
            }
        }
        private void OnParticleCollisionCallback(ParticlePlayer player, GameObject other)
        {
            shellCaseSoundPlayer.Play2D();
        }
        [SerializeField]
        private ParticlePlayer shellCaseParticle;
        [SerializeField]
        private SoundPlayer shellCaseSoundPlayer;
    }
}
