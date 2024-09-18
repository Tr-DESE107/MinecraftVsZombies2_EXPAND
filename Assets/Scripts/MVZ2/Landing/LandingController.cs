using UnityEngine;

namespace MVZ2.Landing
{
    public class LandingController : MonoBehaviour
    {
        public void Display()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void EnterTitleScreen()
        {
            Hide();
            MainManager.Instance.Scene.ShowTitlescreen();
        }
    }
}
