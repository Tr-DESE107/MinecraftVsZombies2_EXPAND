using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Rendering
{
    public class StarshardRotationSetter : ModelComponent
    {
        public override void OnPropertySet(string name, object value)
        {
            base.OnPropertySet(name, value);
            if (value is not Vector3 vector3)
                return;
            switch (name)
            {
                case "Ring1Rotation":
                    ringRoot1.localEulerAngles = vector3;
                    break;
                case "Ring2Rotation":
                    ringRoot2.localEulerAngles = vector3;
                    break;
            }
        }
        [SerializeField]
        private Transform ringRoot1;
        [SerializeField]
        private Transform ringRoot2;
    }
}
