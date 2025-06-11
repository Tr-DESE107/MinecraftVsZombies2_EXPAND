﻿using UnityEngine;

namespace MVZ2.Models
{
    public class WitherModel : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var offset = Model.GetProperty<Vector2>("Offset");
            offset += armorOffsetSpeed;
            offset.x %= 1;
            offset.y %= 1;
            Model.SetProperty("Offset", offset);
        }
        [SerializeField]
        private Vector2 armorOffsetSpeed = new Vector2(0.03f, 0.01f);
    }
}
