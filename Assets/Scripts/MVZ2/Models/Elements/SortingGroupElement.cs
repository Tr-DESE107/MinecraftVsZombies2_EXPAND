﻿using UnityEngine.Rendering;

namespace MVZ2.Models
{
    public class SortingGroupElement : GraphicElement
    {
        public SortingGroup Group
        {
            get
            {
                if (!_group)
                {
                    _group = GetComponent<SortingGroup>();
                }
                return _group;
            }
        }
        public override SerializableGraphicElement ToSerializable()
        {
            return new SerializableSortinGroupElement();
        }
        private SortingGroup _group;

    }
    public class SerializableSortinGroupElement : SerializableGraphicElement
    {
    }
}
