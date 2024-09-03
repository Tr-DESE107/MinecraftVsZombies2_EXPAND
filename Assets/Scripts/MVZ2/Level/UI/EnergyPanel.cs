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
