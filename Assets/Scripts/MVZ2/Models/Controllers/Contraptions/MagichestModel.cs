﻿using Tools;
using UnityEngine;

namespace MVZ2.Models
{
    public class MagichestModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var flashScale = Model.GetProperty<Vector3>("FlashScale");
            flashScale = flashScale * (1 - scaleSpeed) + targetFlashScale * scaleSpeed;
            Model.SetProperty("FlashScale", flashScale);
            flashRootTransform.localScale = Lawn2TransScale(flashScale);

            var sourcePosition = Model.GetProperty<Vector3>("FlashSourcePosition");
            sourcePosition = Lawn2TransPosition(sourcePosition);
            flashTransitor.setStartPosition(sourcePosition);
        }
        [SerializeField]
        private Transform flashRootTransform;
        [SerializeField]
        private PositionTransitor flashTransitor;
        [SerializeField]
        private float scaleSpeed = 0.25f;
        [SerializeField]
        private Vector3 targetFlashScale = new Vector3(25, 25, 25);
    }
}
