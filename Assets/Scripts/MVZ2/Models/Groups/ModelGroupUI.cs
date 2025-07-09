﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Models
{
    [RequireComponent(typeof(Canvas))]
    public class ModelGroupUI : ModelGroup
    {
        #region 着色器
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
        #endregion

        #region 元素管理
        public override void AddElement(GraphicElement element)
        {
            if (element is not ImageElement imageElement)
                throw new ArgumentException("Wrong model group element type.", nameof(element));
            images.Add(imageElement);
        }
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newImages = GetComponentsInChildren<Image>(true)
                .Where(g => g.IsDirectChild<ModelGroup>(this) && !g.GetComponent<Mask>())
                .Select(r =>
                {
                    var element = r.GetComponent<ImageElement>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<ImageElement>();
                    }
                    return element;
                });
            images.ReplaceList(newImages);
        }
        #endregion

        #region 序列化
        public override SerializableModelGroup ToSerializable()
        {
            var ui = new SerializableModelGroupUI();
            SaveToSerializableGroup(ui);
            return ui;
        }
        #endregion

        #region 属性字段
        [SerializeField]
        private List<ImageElement> images = new List<ImageElement>();
        #endregion
    }
    public class SerializableModelGroupUI : SerializableModelGroup
    {
    }
}