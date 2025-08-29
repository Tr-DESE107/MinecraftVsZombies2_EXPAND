using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.waterStain)]
    public class WaterStain : EffectBehaviour
    {
        public WaterStain(string nsp, string name) : base(nsp, name)
        {
            AddAura(new SlideAura());
            AddAura(new WetGridAura());
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_DISPLAY_SCALE_MULTIPLIER));
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULTIPLIER));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var collisionMask = EntityCollisionHelper.MASK_ALL;
            entity.CollisionMaskFriendly = collisionMask;
            entity.CollisionMaskHostile = collisionMask;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var t = Mathf.Clamp01(entity.Timeout / (float)Ticks.FromSeconds(MAX_FADE_SECONDS));
            var colorMulti = new Color(1, 1, 1, t);
            var scaleMulti = Vector3.one * t;
            entity.SetProperty(PROP_TINT_MULTIPLIER, colorMulti);
            entity.SetProperty(PROP_DISPLAY_SCALE_MULTIPLIER, scaleMulti);
            entity.SetModelProperty("Frozen", IsStainFrozen(entity));
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            if (collision.Other.IsFrost())
            {
                var stain = collision.Entity;
                FreezeStain(stain);
            }
            else if (collision.Other.IsFire())
            {
                var stain = collision.Entity;
                MeltStain(stain);
                Disappear(stain);
            }
        }
        public static void FreezeStain(Entity stain)
        {
            var buffID = VanillaBuffID.Effect.waterStainFrozen;
            var buff = stain.GetFirstBuff(buffID);
            if (buff == null)
            {
                buff = stain.AddBuff(buffID);
            }
            WaterStainFrozenBuff.ResetTimeout(buff);
        }
        public static void MeltStain(Entity stain)
        {
            var buffID = VanillaBuffID.Effect.waterStainFrozen;
            stain.RemoveBuffs(buffID);
        }
        public static bool IsStainFrozen(Entity stain)
        {
            return stain.HasBuff(VanillaBuffID.Effect.waterStainFrozen);
        }
        public static Entity UpdateStain(LevelEngine level, Vector3 position, Entity spawner)
        {
            var foundStain = FindStainAtPosition(level, position);
            if (foundStain.ExistsAndAlive())
            {
                foundStain.Timeout = foundStain.GetMaxTimeout();
                return foundStain;
            }
            else
            {
                return level.Spawn(VanillaEffectID.waterStain, position, spawner);
            }
        }
        public static Entity FindStainAtPosition(LevelEngine level, Vector3 position)
        {
            var bounds = GetDetectionBounds(position);

            var mask = EntityCollisionHelper.MASK_EFFECT;
            resultsBuffer.Clear();
            level.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, mask, mask, resultsBuffer);
            return resultsBuffer.FirstOrDefault(c => c.Entity.IsEntityOf(VanillaEffectID.waterStain) && !IsDisappearing(c.Entity))?.Entity;
        }
        public static void Disappear(Entity stain)
        {
            stain.Timeout = Mathf.Min(Ticks.FromSeconds(MAX_FADE_SECONDS), stain.Timeout);
        }
        public static bool IsDisappearing(Entity stain)
        {
            return stain.Timeout <= Ticks.FromSeconds(MAX_FADE_SECONDS);
        }
        private static Bounds GetDetectionBounds(Vector3 position)
        {
            var center = position;
            var def = Global.Game.GetEntityDefinition(VanillaEffectID.waterStain);
            var size = def.GetSize() * 0.5f;
            size.y = 800;
            return new Bounds(center, size);
        }
        public const float MAX_FADE_SECONDS = 0.5f;
        private static List<IEntityCollider> resultsBuffer = new List<IEntityCollider>();
        public static readonly VanillaEntityPropertyMeta<Vector3> PROP_DISPLAY_SCALE_MULTIPLIER = new VanillaEntityPropertyMeta<Vector3>("scale_multiplier");
        public static readonly VanillaEntityPropertyMeta<Color> PROP_TINT_MULTIPLIER = new VanillaEntityPropertyMeta<Color>("tint_multiplier");
        public class SlideAura : AuraEffectDefinition
        {
            public SlideAura() : base(VanillaBuffID.Enemy.waterStainSlide, 4)
            {
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect?.Source?.GetEntity();
                if (source == null)
                    return;
                if (IsDisappearing(source))
                    return;
                collisions.Clear();
                source.GetCurrentCollisions(collisions);
                foreach (var collision in collisions)
                {
                    if (!collision.OtherCollider.IsForMain())
                        continue;
                    var other = collision.Other;
                    if (other.Type != EntityTypes.ENEMY)
                        continue;
                    if (other.GetGravity() <= 0 || !other.IsOnGround)
                        continue;
                    results.Add(other);
                }
            }
            private List<EntityCollision> collisions = new List<EntityCollision>();
        }
        public class WetGridAura : AuraEffectDefinition
        {
            public WetGridAura() : base(VanillaBuffID.Grid.waterStainWet, 7)
            {
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect?.Source?.GetEntity();
                if (source == null)
                    return;
                if (IsStainFrozen(source))
                    return;
                results.Add(source.GetGrid());
            }
        }
    }
}