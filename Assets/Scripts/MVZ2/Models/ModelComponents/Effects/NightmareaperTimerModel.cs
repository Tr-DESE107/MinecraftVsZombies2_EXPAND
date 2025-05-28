﻿using System;
using TMPro;
using UnityEngine;

namespace MVZ2.Models
{
    public class NightmareaperTimerModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var timeout = Model.GetProperty<int>("Timeout");
            var timeSpan = TimeSpan.FromSeconds(timeout / 30f);
            var text = timeSpan.ToString(@"mm\:ss\.ff");
            countdownText.text = text;
            countdownText.color = Model.GetProperty<Color>("Color");
        }
        [SerializeField]
        private TextMeshPro countdownText;
    }
}
