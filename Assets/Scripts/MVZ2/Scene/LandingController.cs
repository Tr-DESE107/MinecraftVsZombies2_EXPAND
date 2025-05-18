using MVZ2.Managers;

namespace MVZ2.Scenes
{
    public class LandingController : MainScenePage
    {
        public void EnterTitleScreen()
        {
            MainManager.Instance.Scene.DisplayTitlescreen();
        }
    }
}
