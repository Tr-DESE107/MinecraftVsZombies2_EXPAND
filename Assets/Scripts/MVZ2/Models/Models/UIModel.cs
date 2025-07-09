﻿using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class UIModel : Model
    {
        public override void Init(Camera camera, int seed = 0)
        {
            base.Init(camera, seed);
            canvas.worldCamera = camera;
        }
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newCanvas = GetComponent<Canvas>();
            if (newCanvas != canvas)
            {
                canvas = newCanvas;
            }
            var newGroup = GetComponent<ModelGroupUI>();
            if (newGroup != group)
            {
                group = newGroup;
            }
        }
        protected override SerializableModelData CreateSerializable()
        {
            var serializable = new SerializableUIModelData();
            return serializable;
        }
        public override ModelGroup GraphicGroup => ImageGroup;
        public ModelGroupUI ImageGroup => group;
        [Header("Image")]
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private ModelGroupUI group;
    }
    public class SerializableUIModelData : SerializableModelData
    {
    }
}