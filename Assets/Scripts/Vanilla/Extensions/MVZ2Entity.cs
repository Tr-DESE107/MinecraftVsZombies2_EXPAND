using MVZ2.GameContent;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Entity
    {
        public static void PlayHitSound(this Entity entity, float damageAmount, DamageEffectList damageEffects, ShellDefinition shell)
        {
            var level = entity.Game;
            bool blocksFire = entity.GetProperty<bool>(EntityProperties.BLOCKS_FIRE);
            NamespaceID hitSound = entity.GetProperty<NamespaceID>(EntityProps.HIT_SOUND);
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
