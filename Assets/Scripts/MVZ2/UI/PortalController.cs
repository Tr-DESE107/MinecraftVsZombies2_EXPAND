using System;
using UnityEngine;

namespace MVZ2.UI
{
    public class PortalController : MonoBehaviour
    {
        public void SetDisplay(bool display)
        {
            animator.SetBool("Display", display);
        }
        public void Fadeout()
        {
            animator.SetTrigger("Fadeout");
        }
        public void ResetFadeout()
        {
            animator.ResetTrigger("Fadeout");
        }
        public void CallFadeIn()
        {
            OnFadeIn?.Invoke();
            OnFadeIn = null;
        }
        public event Action OnFadeIn;
        [SerializeField]
        public Animator animator;
    }
}
