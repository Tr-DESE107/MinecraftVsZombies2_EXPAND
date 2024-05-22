using MVZ2.Level;
using UnityEngine;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        public void StartLevel()
        {
            controller.SetMainManager(main);
            controller.StartGame();
        }
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private LevelController controller;
    }
}
