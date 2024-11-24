using UnityEngine;

namespace MVZ2.Scenes
{
    public class MainScenePage : MonoBehaviour
    {
        public virtual void Display()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
