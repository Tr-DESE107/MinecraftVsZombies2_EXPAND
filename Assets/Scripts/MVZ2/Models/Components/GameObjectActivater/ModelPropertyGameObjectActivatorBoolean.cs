using UnityEngine;

namespace MVZ2.Models
{
    public class ModelPropertyGameObjectActivatorBoolean : ModelPropertyGameObjectActivator
    {
        public override bool GetActive()
        {
            return Model.GetProperty<bool>(propertyName);
        }
        [SerializeField]
        private string propertyName;
    }
}