﻿using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererPointLocker : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var renderer = line;
            if (index < 0 || index >= renderer.positionCount)
                return;
            if (!target)
                return;
            var pos = target.position;
            if (!renderer.useWorldSpace)
            {
                pos = renderer.transform.InverseTransformPoint(pos);
            }
            renderer.SetPosition(index, pos);
        }
        private LineRenderer line
        {
            get
            {
                if (!_line)
                {
                    _line = GetComponent<LineRenderer>();
                }
                return _line;
            }
        }
        private LineRenderer _line;
        [SerializeField]
        private int index;
        [SerializeField]
        private Transform target;
    }
}
