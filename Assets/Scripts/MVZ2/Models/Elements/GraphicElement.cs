﻿using UnityEngine;

namespace MVZ2.Models
{
    public abstract class GraphicElement : MonoBehaviour
    {
        public abstract SerializableGraphicElement ToSerializable();
        public virtual void LoadFromSerializable(SerializableGraphicElement serializable) { }
        public bool ExcludedInGroup => excludedInGroup;
        [SerializeField]
        private bool excludedInGroup;
    }
    public class SerializableGraphicElement
    {
    }
}
