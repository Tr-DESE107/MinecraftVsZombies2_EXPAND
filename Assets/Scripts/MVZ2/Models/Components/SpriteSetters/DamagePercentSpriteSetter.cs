﻿using UnityEngine;

namespace MVZ2.Models
{
    public class DamagePercentSpriteSetter : SpriteSetter
    {
        public override int GetIndex()
        {
            var percent = Model.GetProperty<float>("DamagePercent");
            var count = sprites.Length;
            return Mathf.FloorToInt(percent * count);
        }
    }
}