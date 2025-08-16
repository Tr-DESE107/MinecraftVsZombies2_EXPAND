﻿using UnityEngine;

namespace MVZ2.UI
{
    public class SimpleTooltipSource : ITooltipSource
    {
        public SimpleTooltipSource(Camera camera, ITooltipTarget target, TooltipContent viewData)
        {
            this.camera = camera;
            this.target = target;
            this.viewData = viewData;
        }
        public Camera GetCamera() => camera;
        public ITooltipTarget GetTarget() => target;
        public TooltipContent GetContent() => viewData;
        private Camera camera;
        private ITooltipTarget target;
        private TooltipContent viewData;
    }
}
