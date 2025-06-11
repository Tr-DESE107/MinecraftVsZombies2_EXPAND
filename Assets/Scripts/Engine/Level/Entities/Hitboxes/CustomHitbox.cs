﻿using UnityEngine;

namespace PVZEngine.Entities
{
    public class CustomHitbox : Hitbox
    {
        public CustomHitbox(Entity entity) : base(entity)
        {
        }
        public void SetSize(Vector3 value) => size = value;
        public void SetPivot(Vector3 value) => pivot = value;
        public void SetOffset(Vector3 value) => offset = value;
        public override Vector3 GetSize() => size;
        public override Vector3 GetPivot() => pivot;
        public override Vector3 GetOffset() => offset;

        private Vector3 size;
        private Vector3 pivot;
        private Vector3 offset;
    }
}
