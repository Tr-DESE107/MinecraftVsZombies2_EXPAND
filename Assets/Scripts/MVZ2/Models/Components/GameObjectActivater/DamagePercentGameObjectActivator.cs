﻿using UnityEngine;

namespace MVZ2.Models
{
    public class DamagePercentGameObjectActivator : ModelPropertyGameObjectActivator
    {
        public override bool GetActive()
        {
            var percent = Model.GetProperty<float>("DamagePercent");
            var target = numerator / (float)denominator;
            return Compare(percent, target, comparer);
        }
        private bool Compare(float value, float target, FloatComparer comparer)
        {
            switch (comparer)
            {
                case FloatComparer.Greater:
                    return value > target;
                case FloatComparer.Less:
                    return value < target;
            }
            return false;
        }
        [SerializeField]
        private FloatComparer comparer;
        [SerializeField]
        private int numerator;
        [SerializeField]
        private int denominator;
    }
    public enum FloatComparer
    {
        Greater = 0,
        Less = 1,
    }
}