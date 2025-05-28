using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Models
{
    public abstract class ModelGraphicGroup : MonoBehaviour
    {
        #region 公有方法
        public abstract void AddElement(GraphicElement element);
        public void UpdateFrame(float deltaTime)
        {
            foreach (var animator in animators)
            {
                if (testMode && Application.isEditor)
                {
                    animator.enabled = true;
                }
                else
                {
                    animator.enabled = false;
                    animator.Update(deltaTime);
                }
            }
        }
        public virtual void SetSimulationSpeed(float speed)
        {
        }
        public void SetGroundY(float y)
        {
            foreach (var trans in transforms)
            {
                if (!trans.LockToGround)
                    continue;
                var pos = trans.transform.position;
                pos.y = y;
                trans.transform.position = pos;
            }
        }
        public abstract void UpdateElements();
        public abstract void SetShaderInt(string name, int value);
        public abstract void SetShaderFloat(string name, float alpha);
        public abstract void SetShaderColor(string name, Color color);
        public abstract void SetShaderVector(string name, Vector4 vector);
        public void SetTint(Color color)
        {
            SetShaderColor("_Color", color);
        }
        public void SetColorOffset(Color color)
        {
            SetShaderColor("_ColorOffset", color);
        }
        public void SetHSV(Vector3 hsv)
        {
            SetShaderVector("_HSVOffset", hsv);
        }
        public void TriggerAnimator(string name)
        {
            foreach (var animator in animators)
            {
                animator.SetTrigger(name);
            }
        }
        public void SetAnimatorBool(string name, bool value)
        {
            foreach (var animator in animators)
            {
                animator.SetBool(name, value);
            }
        }
        public void SetAnimatorInt(string name, int value)
        {
            foreach (var animator in animators)
            {
                animator.SetInteger(name, value);
            }
        }
        public void SetAnimatorFloat(string name, float value)
        {
            foreach (var animator in animators)
            {
                animator.SetFloat(name, value);
            }
        }

        public SerializableModelGraphicGroup ToSerializable()
        {
            var serializable = CreateSerializable();
            serializable.animators = animators.Select(a => new SerializableAnimator(a)).ToArray();
            serializable.sortingLayerID = SortingLayerID;
            serializable.sortingOrder = SortingOrder;
            return serializable;
        }
        public void LoadFromSerializable(SerializableModelGraphicGroup serializable)
        {
            SortingLayerID = serializable.sortingLayerID;
            SortingOrder = serializable.sortingOrder;
            for (int i = 0; i < animators.Count; i++)
            {
                if (i >= serializable.animators.Length)
                    break;
                var animator = animators[i];
                var data = serializable.animators[i];
                data.Deserialize(animator);
            }
            LoadSerializable(serializable);
        }
        #endregion

        #region 私有方法
        private void Awake()
        {
            foreach (var animator in animators)
            {
                animator.logWarnings = false;
            }
        }
        protected static bool IsChildOfGroup(Transform child, ModelGraphicGroup group)
        {
            return child.GetComponentInParent<ModelGraphicGroup>() == group;
        }
        protected abstract SerializableModelGraphicGroup CreateSerializable();
        protected virtual void LoadSerializable(SerializableModelGraphicGroup serializable)
        {
        }
        #endregion
        public abstract int SortingLayerID { get; set; }
        public abstract string SortingLayerName { get; set; }
        public abstract int SortingOrder { get; set; }


        [SerializeField]
        private bool testMode = false;
        [SerializeField]
        protected List<Animator> animators = new List<Animator>();
        [SerializeField]
        protected List<TransformElement> transforms = new List<TransformElement>();

    }
    public class SerializableModelGraphicGroup
    {
        public SerializableAnimator[] animators;
        public int sortingLayerID;
        public int sortingOrder;
    }
}