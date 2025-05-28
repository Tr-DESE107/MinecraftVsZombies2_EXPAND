﻿using UnityEngine;

namespace PVZEngine.Entities
{
    public class SerializableCustomHitbox : SerializableHitbox
    {
        public SerializableCustomHitbox(CustomHitbox hitbox) : base(hitbox)
        {
            size = hitbox.GetSize();
            pivot = hitbox.GetPivot();
            offset = hitbox.GetOffset();
        }
        public override Hitbox ToDeserialized(Entity entity)
        {
            var hitbox = new CustomHitbox(entity);
            hitbox.SetSize(size);
            hitbox.SetPivot(pivot);
            hitbox.SetOffset(offset);
            DeserializeProperties(hitbox);
            return hitbox;
        }
        public Vector3 size;
        public Vector3 pivot;
        public Vector3 offset;
    }
}
