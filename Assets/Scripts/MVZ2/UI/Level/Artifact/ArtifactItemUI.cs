using System;
using MVZ2.Models;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactItemUI : MonoBehaviour, ITooltipTarget, IPointerEnterHandler, IPointerExitHandler
    {
        public void SetGlowing(bool glowing)
        {
            if (!animator.gameObject.activeInHierarchy)
                return;
            animator.SetBool("Glowing", glowing);
        }
        public void Shine()
        {
            if (!animator.gameObject.activeInHierarchy)
                return;
            animator.SetTrigger("Shine");
        }
        public void SetGrayscale(bool grayscale)
        {
            if (!animator.gameObject.activeInHierarchy)
                return;
            animator.SetBool("Grayscale", grayscale);
        }
        public void SetIcon(Sprite sprite)
        {
            foreach (var icon in iconImages)
            {
                icon.sprite = sprite;
                icon.enabled = icon.sprite;
            }
        }
        public void SetNumber(string number)
        {
            numText.text = number;
        }
        public void UpdateAnimator(float deltaTime)
        {
            if (!animator.gameObject.activeInHierarchy)
                return;
            animator.enabled = false;
            animator.Update(deltaTime);
        }
        public SerializableAnimator GetSerializableAnimator()
        {
            return new SerializableAnimator(animator);
        }
        public void LoadFromSerializableAnimator(SerializableAnimator seri)
        {
            seri.Deserialize(animator);
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        public Action<ArtifactItemUI> OnPointerEnter;
        public Action<ArtifactItemUI> OnPointerExit;

        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Image[] iconImages;
        [SerializeField]
        private TextMeshProUGUI numText;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
        ITooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;
    }
}
