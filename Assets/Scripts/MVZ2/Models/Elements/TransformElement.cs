﻿using UnityEngine;

namespace MVZ2.Models
{
    public class TransformElement : MonoBehaviour
    {
        public bool LockToGround => lockToGround;
        [SerializeField]
        private bool lockToGround;
    }
}
