using UnityEngine;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        private void Start()
        {
            StartLevel();
        }
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
