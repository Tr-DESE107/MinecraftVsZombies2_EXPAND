using UnityEngine;

namespace MVZ2.Level.UI
{
    public class LevelHintText : MonoBehaviour
    {
        public void SetActive(bool visible)
        {
            gameObject.SetActive(visible);
        }
        public Animator TextAnimator => textAnimator;
        [SerializeField]
        private Animator textAnimator;
    }
}
