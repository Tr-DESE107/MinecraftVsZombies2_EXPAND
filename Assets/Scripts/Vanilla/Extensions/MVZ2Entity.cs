using MVZ2.GameContent;
using MVZ2.GameContent.Shells;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Entity
    {
        public static void PlayHitSound(this Entity entity, DamageEffectList damageEffects, ShellDefinition shell)
        {
            if (entity == null || shell == null)
                return;
            var level = entity.Level;
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
