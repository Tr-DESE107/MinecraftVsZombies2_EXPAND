using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.hellfire)]
    public class Hellfire : ContraptionBehaviour
    {
        public Hellfire(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskFriendly |= EntityCollisionHelper.MASK_PROJECTILE;
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationBool("Evoked", IsCursed(entity));
        }
        public override bool CanEvoke(Entity entity)
        {
            if (IsCursed(entity))
                return false;
            var meteor = GetMeteor(entity);
            if (meteor != null && meteor.Exists(entity.Level))
                return false;
            return base.CanEvoke(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var pos = entity.Position + new Vector3(0, 1280, 0);
            var meteor = entity.SpawnWithParams(VanillaEffectID.cursedMeteor, pos);
            meteor.SetParent(entity);
            SetMeteor(entity, new EntityID(meteor));
            meteor.PlaySound(VanillaSoundID.bombFalling);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            var other = collision.Other;
            if (!other.IsEntityOf(VanillaProjectileID.arrow))
                return;
            var self = collision.Entity;
            if (!self.IsFriendly(other))
                return;
            if (other.HasBuff<HellfireIgnitedBuff>())
                return;
            var buff = other.AddBuff<HellfireIgnitedBuff>();
            if (IsCursed(self))
            {
                HellfireIgnitedBuff.Curse(buff);
            }
        }
        public static void Curse(Entity entity)
        {
            SetCursed(entity, true);
            entity.AddBuff<HellfireCursedBuff>();
        }
        public static void SetCursed(Entity entity, bool value) => entity.SetProperty(PROP_CURSED, value);
        public static bool IsCursed(Entity entity) => entity.GetProperty<bool>(PROP_CURSED);
        public static void SetMeteor(Entity entity, EntityID value) => entity.SetProperty(PROP_METEOR, value);
        public static EntityID GetMeteor(Entity entity) => entity.GetProperty<EntityID>(PROP_METEOR);
        public static readonly VanillaBuffPropertyMeta PROP_CURSED = new VanillaBuffPropertyMeta("cursed");
        public static readonly VanillaBuffPropertyMeta PROP_METEOR = new VanillaBuffPropertyMeta("meteor");
    }
}
