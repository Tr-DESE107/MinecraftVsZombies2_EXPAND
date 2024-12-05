using PVZEngine;
using UnityEngine;

namespace MVZ2.Map
{
    public class MapButtonBorder : MonoBehaviour
    {
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
        public NamespaceID difficulty;
    }
}
