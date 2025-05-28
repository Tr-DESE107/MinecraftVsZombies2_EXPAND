﻿using PVZEngine.Armors;
using PVZEngine.Entities;

namespace PVZEngine.Damages
{
    public class ArmorDestroyInfo
    {
        public DamageEffectList Effects { get; private set; }
        public Entity Entity { get; private set; }
        public EntityReferenceChain Source { get; set; }
        public ArmorDamageResult Damage { get; private set; }
        public Armor Armor { get; private set; }
        public NamespaceID Slot { get; private set; }

        public ArmorDestroyInfo(Entity entity, Armor armor, NamespaceID slot, DamageEffectList effects, EntityReferenceChain source, ArmorDamageResult damage)
        {
            Effects = effects;
            Entity = entity;
            Armor = armor;
            Slot = slot;
            Source = source;
            Damage = damage;
        }
        public bool HasEffect(NamespaceID effect)
        {
            if (Effects == null)
                return false;
            return Effects.HasEffect(effect);
        }
    }
}
