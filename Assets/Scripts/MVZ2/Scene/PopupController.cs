using TMPro;
using UnityEngine;

namespace MVZ2.Scenes
{
    public class PopupController : ScenePage
    {
        public void ShowPopup(string text)
        {
            animator.SetTrigger("Show");
            popupText.text = text;
        }
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private TextMeshProUGUI popupText;
    }
}
