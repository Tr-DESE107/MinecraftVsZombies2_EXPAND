﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ModelPropertyGameObjectActivatorBoolean : ModelPropertyGameObjectActivator
    {
        public override bool GetActive()
        {
            return Model.GetProperty<bool>(propertyName) != whenFalse;
        }
        [SerializeField]
        private bool whenFalse;
        [SerializeField]
        private string propertyName;
    }
}