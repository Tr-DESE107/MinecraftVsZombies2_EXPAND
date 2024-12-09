using TMPro;
using UnityEngine;

namespace MVZ2.Talk
{
    public class SpeechBubble : MonoBehaviour
    {
        public void SetShowing(bool showing)
        {
            animator.SetBool("Show", showing);
        }
        public void ForceReshow()
        {
            animator.SetTrigger("Reshow");
        }
        public void SetDirection(SpeechBubbleDirection direction)
        {
            animator.SetInteger("Direction", (int)direction);
        }
        public void SetText(string text)
        {
            talkText.text = text;
        }
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private TextMeshProUGUI talkText;
    }
    public enum SpeechBubbleDirection
    {
        Right,
        Down,
        Left,
        Up,
    }
}
