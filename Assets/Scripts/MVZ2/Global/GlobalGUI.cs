using System;
using MVZ2.Managers;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.GlobalGame
{
    public class GlobalGUI : IGlobalGUI
    {
        public GlobalGUI(MainManager main)
        {
            this.main = main;
        }
        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect = null)
        {
            main.Scene.ShowDialog(title, desc, options, onSelect);
        }
        private MainManager main;

    }
}
