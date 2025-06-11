﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ModelPropertySpriteSetterInt : SpriteSetter
    {
        public override int GetIndex()
        {
            return Model.GetProperty<int>(propertyName);
        }
        [SerializeField]
        private string propertyName;
    }
}