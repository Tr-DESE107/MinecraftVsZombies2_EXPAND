using UnityEngine;
using MVZ2.GameContent;
using PVZEngine;
using MVZ2.GameContent.Shells;

namespace MVZ2.Vanilla
{
    public static class MVZ2Entity
    {
        public static Vector3 GetSize(this EntityDefinition definition)
        {
            return definition.GetProperty<Vector3>(EntityProperties.SIZE);
        }
        public static NamespaceID GetPlaceSound(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(EntityProps.PLACE_SOUND);
        }
        public static NamespaceID GetDeathSound(this Entity entity)
        {
            return entity.GetProperty<NamespaceID>(EntityProps.DEATH_SOUND);
        }
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Game;
            var blocksFire = shell.GetProperty<bool>(ShellProps.BLOCKS_FIRE);
            var hitSound = shell.GetProperty<NamespaceID>(ShellProps.HIT_SOUND);
            if (damageEffects.HasEffect(DamageEffects.FIRE) && !blocksFire)
            {
                level.PlaySound(SoundID.fire, entity.Pos);
            }
            else if (damageEffects.HasEffect(DamageEffects.SLICE) && shell.GetProperty<bool>(ShellProps.SLICE_CRITICAL))
            {
                level.PlaySound(SoundID.slice, entity.Pos);
            }
            else
            {
                level.PlaySound(hitSound, entity.Pos);
            }
        }
    }
}
