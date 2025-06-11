﻿using TMPro;
using UnityEngine;

namespace MVZ2.Models
{
    public class FloatingTextModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            text.text = Model.GetProperty<string>("Text");
            text.color = Model.GetProperty<Color>("Color");
        }
        [SerializeField]
        private TextMeshPro text;
    }
}
