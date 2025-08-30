using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.Windows;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.zombieCloud)]
    public class ZombieCloud : StateEnemy
    {
        public ZombieCloud(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_APPLY_STATUS_EFFECT, PreEntitySlowCallback, filter: VanillaBuffID.Entity.slow);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 80f);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var variant = entity.GetVariant();
            if (variant == ZombieCloud.VARIANT_NORMAL || variant == ZombieCloud.VARIANT_THUNDER)
            {
                if (entity.IsSecondsInterval(0.25f))
                {
                    var x = entity.RNG.NextFloat() * 32f - 16f;
                    var pos = entity.Position + new Vector3(x, 0, 0);
                    entity.Spawn(VanillaEffectID.zombieCloudRaindrop, pos);
                }
            }
            if (variant == ZombieCloud.VARIANT_THUNDER)
            {
                if (entity.IsSecondsInterval(3f))
                {
                    var position = entity.Position;
                    position.y = entity.Level.GetGroundY(position);
                    TeslaCoil.Shock(entity, entity.GetDamage() * THUNDER_DAMAGE_MULTIPLIER, entity.GetFaction(), THUNDER_SHOCK_RADIUS, position);
                    TeslaCoil.CreateArc(entity, entity.Position, position);
                    entity.PlaySound(VanillaSoundID.thunder);
                }
            }
            if (variant == ZombieCloud.VARIANT_SNOW)
            {
                if (entity.IsSecondsInterval(0.25f))
                {
                    var x = entity.RNG.NextFloat() * 32f - 16f;
                    var pos = entity.Position + new Vector3(x, 0, 0);
                    entity.Spawn(VanillaEffectID.zombieCloudSnowflake, pos);
                }
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("Variant", entity.GetVariant());
        }
        public override void PreTakeDamage(DamageInput input, CallbackResult result)
        {
            base.PreTakeDamage(input, result);
            if (input.HasEffect(VanillaDamageEffects.LIGHTNING))
            {
                ChangeVariant(input.Entity, VARIANT_THUNDER);
                input.Multiply(0);
                result.Break();
                return;
            }
            if (input.HasEffect(VanillaDamageEffects.ICE))
            {
                ChangeVariant(input.Entity, VARIANT_SNOW);
                input.Multiply(0);
                result.Break();
                return;
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.TINT, GetSmokeColor(entity));
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter(), param);
            entity.Remove();
        }
        public static void ChangeVariant(Entity entity, int variant)
        {
            if (entity.GetVariant() == variant)
                return;
            entity.SetVariant(variant);
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.TINT, GetSmokeColor(entity));
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter(), param);
        }
        private static Color GetSmokeColor(Entity entity)
        {
            switch (entity.GetVariant())
            {
                case VARIANT_THUNDER:
                    return SMOKE_COLOR_THUNDER;
                case VARIANT_SNOW:
                    return SMOKE_COLOR_SNOW;
            }
            return SMOKE_COLOR_NORMAL;
        }
        private void PreEntitySlowCallback(VanillaLevelCallbacks.PreApplyStatusEffectParams param, CallbackResult result)
        {
            var entity = param.entity;
            if (!entity.IsEntityOf(VanillaEnemyID.zombieCloud))
                return;
            ChangeVariant(entity, VARIANT_SNOW);
            result.SetFinalValue(false);
        }
        public const int VARIANT_NORMAL = 0;
        public const int VARIANT_THUNDER = 1;
        public const int VARIANT_SNOW = 2;

        public const float THUNDER_DAMAGE_MULTIPLIER = 1f;
        public const float THUNDER_SHOCK_RADIUS = 20f;
        public static readonly Color SMOKE_COLOR_NORMAL = new Color32(241, 191, 227, 255);
        public static readonly Color SMOKE_COLOR_THUNDER = new Color32(54, 54, 54, 255);
        public static readonly Color SMOKE_COLOR_SNOW = new Color32(191, 228, 241, 255);
    }
}
