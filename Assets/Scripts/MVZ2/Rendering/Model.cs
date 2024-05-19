using System;
using System.Collections.Generic;
using Rendering;
using UnityEngine;

namespace MVZ2
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法
        public void UpdateView(float deltaTime)
        {
            Animator.enabled = false;
            Animator.Update(deltaTime);
        }
        public void UpdateLogic()
        {
            triggeringEvents.Clear();
        }
        public void TriggerAnimator(string name)
        {
            Animator.SetTrigger(name);
        }
        public void SetAnimatorBool(string name, bool value)
        {
            Animator.SetBool(name, value);
        }
        public void SetAnimatorInt(string name, int value)
        {
            Animator.SetInteger(name, value);
        }
        public void SetAnimatorFloat(string name, float value)
        {
            Animator.SetFloat(name, value);
        }
        public bool IsEventTriggered(string name)
        {
            return triggeringEvents.Contains(name);
        }

        public bool WasEventTriggered(string name)
        {
            return triggeredEvents.Contains(name);
        }
        #endregion

        #region 私有方法
        private void TriggerEvent(string name)
        {
            triggeringEvents.Add(name);
            triggeredEvents.Add(name);
        }
        #endregion


        #region 属性字段
        public MultipleRendererGroup RendererGroup => rendererGroup;
        public Animator Animator => animator;
        public Transform RootTransform => rootTransform;
        public float AnimationSpeed { get; set; }
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform centerTransform;
        private List<string> triggeringEvents = new List<string>();
        private List<string> triggeredEvents = new List<string>();
        #endregion
    }
}