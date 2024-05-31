using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class EnergyPanel : LevelUIUnit
    {
        public void SetEnergy(string energy)
        {
            energyText.text = energy;
        }
        [SerializeField]
        private TextMeshProUGUI energyText;
    }
}
