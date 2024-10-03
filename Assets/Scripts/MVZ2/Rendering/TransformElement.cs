using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Rendering
{
    public class TransformElement : MonoBehaviour
    {
        public bool LockToGround => lockToGround;
        [SerializeField]
        private bool lockToGround;
    }
}
