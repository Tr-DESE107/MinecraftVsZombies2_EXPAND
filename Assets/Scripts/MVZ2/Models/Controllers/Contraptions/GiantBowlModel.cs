﻿using MVZ2.UI;
using Tools;
using UnityEngine;

namespace MVZ2.Models
{
    public class GiantBowlModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var count = Model.GetProperty<int>("Count");
            var angle = Model.GetProperty<float>("Angle");
            var radial = Model.GetProperty<float>("Radial");

            if (pointList.Count != count)
            {
                pointList.updateList(count);
            }
            var radius = Mathf.Lerp(minRadius, maxRadius, radial);
            for (int i = 0; i < count; i++)
            {
                var point = pointList.getElement(i);

                var off2D = Vector2.up.RotateClockwise(angle + i * (360 / (float)count)) * radius;
                off2D.y *= circleAspect;
                point.transform.localPosition = new Vector3(off2D.x, off2D.y);
            }
        }
        [SerializeField]
        private ElementList pointList;
        [SerializeField]
        private float maxRadius = 0.64f;
        [SerializeField]
        private float minRadius = 0.32f;
        [SerializeField]
        private float circleAspect = 0.5f;
    }
}
