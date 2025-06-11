﻿using TMPro;
using UnityEngine;

namespace MVZ2.Models
{
    public class BreakoutBoardModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var countdown = Model.GetProperty<int>("Countdown");
            string text = string.Empty;
            if (countdown > 0)
            {
                var seconds = Mathf.CeilToInt(countdown / 30f);
                text = seconds.ToString();
            }
            countdownText.text = text;
        }
        [SerializeField]
        private TextMeshPro countdownText;
    }
}
