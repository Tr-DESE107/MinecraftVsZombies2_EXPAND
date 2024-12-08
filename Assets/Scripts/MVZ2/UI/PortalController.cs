using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class PortalController : MonoBehaviour
    {
        public void SetAlpha(float value)
        {
            fader.Value = value;
        }
        public void StartFade(float target, float duration)
        {
            fader.StartFade(target, duration);
        }
        private void Awake()
        {
            fader.OnValueChanged += OnValueChangedCallback;
            fader.OnFadeFinished += OnFadeFinishedCallback;
        }
        private void Update()
        {
            raycastBlocker.raycastTarget = fader.EndValue > fader.StartValue || fader.Value > 0.85f;
        }
        private void OnValueChangedCallback(float value)
        {
            animator.SetFloat("Blend", value);
        }
        private void OnFadeFinishedCallback(float value)
        {
            OnFadeFinished?.Invoke(value);
        }
        public event Action<float> OnFadeFinished;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Image raycastBlocker;
        [SerializeField]
        private FloatFader fader;
    }
}
