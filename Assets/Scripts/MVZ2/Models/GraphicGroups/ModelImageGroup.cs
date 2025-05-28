using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Models
{
    [RequireComponent(typeof(Canvas))]
    public class ModelImageGroup : ModelGraphicGroup
    {
        #region 公有方法
        public void SetCamera(Camera camera)
        {
            canvas.worldCamera = camera;
        }
        public override void AddElement(GraphicElement element)
        {
            if (element is not ImageElement imageElement)
                throw new ArgumentException("Wrong model group element type.", nameof(element));
            images.Add(imageElement);
        }
        public override void UpdateElements()
        {
            images.Clear();
            foreach (var renderer in GetComponentsInChildren<Image>(true))
            {
                if (!IsChildOfGroup(renderer.transform, this))
                    continue;
                if (renderer.GetComponent<Mask>())
                    continue;
                var element = renderer.GetComponent<ImageElement>();
                if (!element)
                {
                    element = renderer.gameObject.AddComponent<ImageElement>();
                }
                images.Add(element);
            }
            transforms.Clear();
            foreach (var trans in GetComponentsInChildren<TransformElement>(true))
            {
                if (!IsChildOfGroup(trans.transform, this))
                    continue;
                transforms.Add(trans);
            }
            animators.Clear();
            foreach (var animator in GetComponentsInChildren<Animator>(true))
            {
                if (!IsChildOfGroup(animator.transform, this))
                    continue;
                animators.Add(animator);
            }
        }
        public override void SetShaderInt(string name, int value)
        {
        }

        public override void SetShaderFloat(string name, float alpha)
        {
        }

        public override void SetShaderColor(string name, Color color)
        {
        }
        public override void SetShaderVector(string name, Vector4 color)
        {
        }
        protected override SerializableModelGraphicGroup CreateSerializable()
        {
            var serializable = new SerializableModelImageGroup();
            serializable.images = images.Select(e => e.ToSerializable()).ToArray();
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelGraphicGroup serializable)
        {
            base.LoadSerializable(serializable);
            if (serializable is not SerializableModelImageGroup imageGroup)
                return;
            for (int i = 0; i < images.Count; i++)
            {
                if (i >= imageGroup.images.Length)
                    break;
                var element = images[i];
                var data = imageGroup.images[i];
                element.LoadFromSerializable(data);
            }
        }
        #endregion

        #region 私有方法
        private ImageElement[] GetAllElements(bool includeExcluded = false)
        {
            if (includeExcluded)
                return images.ToArray();
            return images.Where(e => !e.ExcludedInGroup).ToArray();
        }
        #endregion

        public override int SortingLayerID { get => canvas.sortingLayerID; set => canvas.sortingLayerID = value; }
        public override string SortingLayerName { get => canvas.sortingLayerName; set => canvas.sortingLayerName = value; }
        public override int SortingOrder { get => canvas.sortingOrder; set => canvas.sortingOrder = value; }

        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private List<ImageElement> images = new List<ImageElement>();

    }
    public class SerializableModelImageGroup : SerializableModelGraphicGroup
    {
        public SerializableGraphicElement[] images;
    }
}