﻿using UnityEngine;

namespace Rendering
{
    [ExecuteAlways]
    public class PositionLocker : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (localSpace)
            {
                transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }
        }
        [SerializeField]
        private bool localSpace = false;
        [SerializeField]
        private Vector3 position = Vector3.zero;
    }
}