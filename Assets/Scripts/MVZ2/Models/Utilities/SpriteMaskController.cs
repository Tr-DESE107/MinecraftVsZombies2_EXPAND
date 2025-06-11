﻿using UnityEngine;

namespace MVZ2.Models
{
    public class SpriteMaskController : MonoBehaviour
    {
        private void Awake()
        {
            spriteMask.materials = replacementMaterials;
        }
        [SerializeField]
        private SpriteMask spriteMask;
        [SerializeField]
        private Material[] replacementMaterials;
    }
}
