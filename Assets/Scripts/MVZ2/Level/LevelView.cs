using UnityEngine;

namespace MVZ2.Level
{
    public class LevelView : MonoBehaviour
    {
        public void SetDoorVisible(bool visible)
        {
            foreach (var obj in doorObjects)
            {
                obj.SetActive(visible);
            }
        }
        [SerializeField]
        private GameObject[] doorObjects;
    }
}
