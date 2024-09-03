using TMPro;
using UnityEngine;

namespace MVZ2.Talk
{
    public class SpeechBubble : MonoBehaviour
    {

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
        public void UpdateBubble(Vector2 position, Vector2 pivot, SpeechBubbleDirection direction)
        {
            var rectTrans = transform as RectTransform;
            rectTrans.anchoredPosition = position;
            rectTrans.pivot = new Vector2(0.5f, 0);
            noArrowImageObj.SetActive(direction == SpeechBubbleDirection.None);
            leftArrowImageObj.SetActive(direction == SpeechBubbleDirection.Left);
            rightArrowImageObj.SetActive(direction == SpeechBubbleDirection.Right);
            downArrowImageObj.SetActive(direction == SpeechBubbleDirection.Down);
        }
        public void SetText(string text)
        {
            talkText.text = text;
        }
        public void ResetScale()
        {
            transform.localScale = Vector3.zero;
        }
        private void Update()
        {
            EnlargeBubble(Time.deltaTime * 16);
        }
        /// <summary>
        /// 放大对话气泡大小。
        /// </summary>
        /// <param name="speed">放大速度，1为常速。</param>
        private void EnlargeBubble(float speed)
        {
            float scale = transform.localScale.y;
            if (scale < 1)
            {
                scale += speed;
                scale = Mathf.Clamp01(scale);
            }
            // 如果气泡未完全放大，则不显示文字。
            talkTextRoot.SetActive(scale >= 1);
            transform.localScale = Vector3.one * scale;
        }
        [SerializeField]
        private GameObject noArrowImageObj;
        [SerializeField]
        private GameObject leftArrowImageObj;
        [SerializeField]
        private GameObject rightArrowImageObj;
        [SerializeField]
        private GameObject downArrowImageObj;
        [SerializeField]
        private TextMeshProUGUI talkText;
        [SerializeField]
        private GameObject talkTextRoot;
    }
    public enum SpeechBubbleDirection
    {
        None,
        Left,
        Right,
        Down,
    }
    public enum SpeechBubblePosition
    {
        Left,
        Right,
        Self,
        Top,
        Bottom
    }
}
