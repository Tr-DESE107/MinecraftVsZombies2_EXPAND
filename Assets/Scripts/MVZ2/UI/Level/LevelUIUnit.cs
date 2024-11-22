using UnityEngine;

namespace MVZ2.Level.UI
{
    public class LevelUIUnit : MonoBehaviour
    {
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        public void MoveIntoScreen()
        {
            uiAnimator.SetTrigger("MoveIntoScreen");
        }
        [SerializeField]
        private Animator uiAnimator;
    }
}
