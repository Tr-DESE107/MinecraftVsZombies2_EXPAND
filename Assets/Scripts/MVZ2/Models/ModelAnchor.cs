﻿using UnityEngine;

namespace MVZ2.Models
{
    public class ModelAnchor : MonoBehaviour
    {
        public string key;
        public int KeyHash { get; private set; }
        private void Awake()
        {
            KeyHash = key.GetHashCode();
        }
    }
}
