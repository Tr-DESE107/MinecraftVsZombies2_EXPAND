﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ModelPropertyGameObjectActivatorInt : ModelPropertyGameObjectActivator
    {
        public override bool GetActive()
        {
            var value = Model.GetProperty<int>(propertyName);
            return Compare(value, constValue, comparer);
        }
        private bool Compare(int value, int target, IntComparer comparer)
        {
            switch (comparer)
            {
                case IntComparer.Equals:
                    return value == target;
                case IntComparer.NotEqual:
                    return value != target;
                case IntComparer.Greater:
                    return value > target;
                case IntComparer.Less:
                    return value < target;
                case IntComparer.GEqual:
                    return value >= target;
                case IntComparer.LEqual:
                    return value <= target;
            }
            return false;
        }
        [SerializeField]
        private string propertyName;
        [SerializeField]
        private IntComparer comparer;
        [SerializeField]
        private int constValue;
    }
    public enum IntComparer
    {
        Equals = 0,
        NotEqual = 1,
        Greater = 2,
        Less = 3,
        GEqual = 4,
        LEqual = 5
    }
}