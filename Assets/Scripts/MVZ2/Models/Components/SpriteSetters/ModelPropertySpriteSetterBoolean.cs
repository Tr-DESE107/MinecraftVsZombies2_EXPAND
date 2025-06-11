﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ModelPropertySpriteSetterBoolean : SpriteSetter
    {
        public override int GetIndex()
        {
            return Model.GetProperty<bool>(propertyName) ? 1 : 0;
        }
        [SerializeField]
        private string propertyName;
    }
}