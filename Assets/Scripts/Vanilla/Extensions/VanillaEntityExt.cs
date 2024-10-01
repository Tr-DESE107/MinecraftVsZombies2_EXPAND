using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Shells;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaEntityExt
    {
        public static void UpdateModelLight(this Entity entity)
        {
            entity.SetModelProperty(BuiltinModelProps.LIGHT_VISIBLE, entity.IsLightSource());
            entity.SetModelProperty(BuiltinModelProps.LIGHT_RANGE, entity.GetLightRange() * Global.LawnToTransScale);
            entity.SetModelProperty(BuiltinModelProps.LIGHT_COLOR, entity.GetLightColor());
        }
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Level;
            var blocksFire = shell.GetProperty<bool>(VanillaShellProps.BLOCKS_FIRE);
            var hitSound = shell.GetProperty<NamespaceID>(VanillaShellProps.HIT_SOUND);
            if (damageEffects.HasEffect(VanillaDamageEffects.FIRE) && !blocksFire)
            {
                entity.PlaySound(SoundID.fire);
            }
            else if (damageEffects.HasEffect(VanillaDamageEffects.SLICE) && shell.GetProperty<bool>(VanillaShellProps.SLICE_CRITICAL))
            {
                entity.PlaySound(SoundID.slice);
            }
            else
            {
                entity.PlaySound(hitSound);
            }
        }
        public static Vector3 ModifyProjectileVelocity(this Entity entity, Vector3 velocity)
        {
            return velocity;
        }
        public static bool IsAliveEnemy(this Entity entity)
        {
            return entity.Type == EntityTypes.ENEMY && !entity.IsDead && !entity.GetProperty<bool>(BuiltinEnemyProps.HARMLESS) && entity.IsEnemy(entity.Level.Option.LeftFaction);
        }
    }
}
