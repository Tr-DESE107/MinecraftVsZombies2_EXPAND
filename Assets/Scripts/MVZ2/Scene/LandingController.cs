using MVZ2.Managers;
using MVZ2Logic.Scenes;

namespace MVZ2.Scenes
{
    public class LandingController : MainScenePage
    {
        public void EnterTitleScreen()
        {
            MainManager.Instance.Scene.DisplayPage(MainScenePageType.Titlescreen);
        }
    }
}
