﻿using UnityEngine;

namespace MVZ2.Models
{
    public class SeijaBulletModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var dark = Model.GetProperty<bool>("Dark");
            if (dark != isDark)
            {
                isDark = dark;
                normalObject.SetActive(!isDark);
                darkObject.SetActive(isDark);
            }
        }
        [SerializeField]
        private GameObject normalObject;
        [SerializeField]
        private GameObject darkObject;
        private bool isDark = false;
    }
}
