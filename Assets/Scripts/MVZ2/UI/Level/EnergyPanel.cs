﻿using TMPro;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class EnergyPanel : LevelUIUnit
    {
        public void FlickerEnergy()
        {
            textAnimator.SetTrigger("Flicker");
        }
        public void SetEnergy(string energy)
        {
            energyText.text = energy;
        }
        [SerializeField]
        private Animator textAnimator;
        [SerializeField]
        private TextMeshProUGUI energyText;
    }
}
