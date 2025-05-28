﻿using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.Entities
{
    public static class VanillaColliderExt
    {
        public static bool IsForMain(this IEntityCollider collider)
        {
            return !NamespaceID.IsValid(collider.ArmorSlot);
        }
        public static bool IsMainCollider(this IEntityCollider collider)
        {
            return collider.Name == EntityCollisionHelper.NAME_MAIN;
        }
        public static DamageInput GetDamageInput(this IEntityCollider collider, float amount, DamageEffectList effects, Entity source)
        {
            return new DamageInput(amount, effects, collider.Entity, new EntityReferenceChain(source), collider.ArmorSlot);
        }
        public static DamageOutput TakeDamage(this IEntityCollider collider, float amount, DamageEffectList effects, Entity source)
        {
            return collider.TakeDamage(amount, effects, new EntityReferenceChain(source));
        }
        public static DamageOutput TakeDamage(this IEntityCollider collider, float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            return collider.Entity.TakeDamage(amount, effects, source, collider.ArmorSlot);
        }
    }
}
