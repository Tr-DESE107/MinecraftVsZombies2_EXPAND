﻿using UnityEngine;

namespace Rendering
{
    [ExecuteAlways]
    public class RotationLocker : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (localSpace)
            {
                transform.localEulerAngles = rotation;
            }
            else
            {
                transform.eulerAngles = rotation;
            }
        }
        [SerializeField]
        private bool localSpace = false;
        [SerializeField]
        private Vector3 rotation = Vector3.zero;
    }
}