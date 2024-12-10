using System.Linq;
using UnityEngine;

namespace MVZ2.Level
{
    public class AreaModel : MonoBehaviour
    {
        public void SetPreset(string name)
        {
            bool hasActive = false;
            foreach (var preset in presets)
            {
                bool active = preset.GetName() == name;
                if (active)
                {
                    hasActive = true;
                }
                preset.SetActive(active);
            }
            if (!hasActive)
            {
                var preset = presets.FirstOrDefault();
                if (preset)
                {
                    preset.SetActive(true);
                }
            }
        }
        public void SetDoorVisible(bool visible)
        {
            foreach (var obj in doorObjects)
            {
                obj.SetActive(visible);
            }
        }
        [SerializeField]
        private GameObject[] doorObjects;
        [SerializeField]
        private AreaModelPreset[] presets;
    }
}
