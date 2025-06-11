﻿using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class CrushingWallsModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            bool isMobile = MainManager.Instance.IsMobile();
            standalone.gameObject.SetActive(!isMobile);
            mobile.gameObject.SetActive(isMobile);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            bool isMobile = MainManager.Instance.IsMobile();
            var platform = isMobile ? mobile : standalone;
            var camera = Model.GetCamera();
            if (!camera)
                return;
            var progress = Model.GetProperty<float>("Progress");
            var shake = Lawn2TransScale(Model.GetProperty<Vector3>("Shake"));
            platform.SetLeftPosition(camera, progress, shake);
            platform.SetRightPosition(camera, progress, shake);
        }
        [SerializeField]
        private CrushingWallsModelPlatform standalone;
        [SerializeField]
        private CrushingWallsModelPlatform mobile;
    }
}
