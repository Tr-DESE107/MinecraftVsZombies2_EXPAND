using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Archives
{
    public class SimulationArchivePage : ArchivePage
    {
        public void SetBackground(Sprite background)
        {
            backgroundImage.sprite = background;
        }
        [SerializeField]
        private Image backgroundImage;
    }
}
