using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class AchievementHint : MonoBehaviour
    {
        public void UpdateAchievement(Sprite icon, string name)
        {
            iconImage.sprite = icon;
            nameText.text = name;
        }
        public void SetShowValue(float value)
        {
            animator.SetFloat("Blend", value);
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke());
        }
        public event Action OnClick;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Button button;
        [SerializeField]
        private TextMeshProUGUI nameText;
    }
}
