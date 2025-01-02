using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class AnimationRotator : MonoBehaviour
    {
        private void Update()
        {
            transform.localEulerAngles = rotation;
        }
        [SerializeField]
        private Vector3 rotation;
    }
}
