namespace MVZ2.Landing
{
    public class LandingController : MainScenePage
    {
        public void EnterTitleScreen()
        {
            MainManager.Instance.Scene.DisplayPage(MainScenePageType.Titlescreen);
        }
    }
}
