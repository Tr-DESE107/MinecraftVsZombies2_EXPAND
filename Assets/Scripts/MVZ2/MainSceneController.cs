using System;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2
{
    public class MainSceneController : MonoBehaviour
    {
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            ui.ShowDialog(title, desc, options, onSelect);
        }
        [SerializeField]
        private MainSceneUI ui;
    }
}
