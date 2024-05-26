using System;
using System.Collections.Generic;
using MVZ2.Rendering;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法
        public void UpdateModel(float deltaTime, float simulationSpeed)
        {
            Animator.enabled = false;
            Animator.Update(deltaTime);
            RendererGroup.SetSimulationSpeed(simulationSpeed);
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
        #region 护甲
        public void SetArmor(Model model)
        {
            if (ArmorModel)
            {
                RemoveArmor();
            }
            if (!model)
                return;
            if (!armorTransform)
                return;
            model.transform.parent = armorTransform;
            model.transform.localPosition = Vector3.zero;
            armorModel = model;
        }
        public void RemoveArmor()
        {
            if (!armorModel)
                return;
            Destroy(armorModel.gameObject);
            armorModel = null;
        }
        #endregion

        #region 属性
        public T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        #endregion

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
        public Transform CenterTransform => centerTransform;
        public Model ArmorModel => armorModel;
        public float AnimationSpeed { get; set; }
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform centerTransform;
        [SerializeField]
        private Transform armorTransform;
        [SerializeField]
        private Model armorModel;
        private List<string> triggeringEvents = new List<string>();
        private List<string> triggeredEvents = new List<string>();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
}