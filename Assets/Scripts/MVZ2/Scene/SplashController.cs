using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class SplashController : ScenePage
    {
        public void EnterTitleScreen()
        {
            MainManager.Instance.Scene.DisplayTitlescreen();
        }
    }
}
