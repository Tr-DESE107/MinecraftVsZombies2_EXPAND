using MVZ2.Managers;

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
