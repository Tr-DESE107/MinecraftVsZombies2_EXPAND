using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.HeldItems
{
    public class SwordHeldItemBehaviour : HeldItemBehaviour
    {
        public SwordHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }
        public override bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            switch (target)
            {
                case HeldItemTargetLawn:
                    return true;
                case HeldItemTargetEntity entityTarget:
                    {
                        var entity = entityTarget.Target;
                        switch (entity.Type)
                        {
                            case EntityTypes.ENEMY:
                                return entity.IsHostileEntity();
                        }
                        return false;
                    }
            }

            return false;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction phase)
        {
            base.Use(target, data, phase);
            switch (target)
            {
                case HeldItemTargetEntity entityTarget:
                    {
                        var entity = entityTarget.Target;

                        switch (entity.Type)
                        {
                            case EntityTypes.ENEMY:
                                if (phase == PointerInteraction.Press)
                                {
                                    if (IsParalyzed(entity.Level))
                                        break;
                                    var effects = new DamageEffectList(VanillaDamageEffects.WHACK, VanillaDamageEffects.REMOVE_ON_DEATH);
                                    entity.TakeDamageNoSource(750, effects);
                                    if (entity.IsDead)
                                    {
                                        var screenPos = Global.GetPointerScreenPosition();
                                        var pos = entity.Level.ScreenToLawnPositionByZ(screenPos, entity.Position.z);
                                        entity.Level.Spawn(VanillaEffectID.pow, pos, null);
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            if (phase == PointerInteraction.Press)
            {
                Swing(target.GetLevel());
            }
        }
        public static void Paralyze(LevelEngine level)
        {
            var buff = level.AddBuff<SwordParalyzedBuff>();
            var timeout = Mathf.FloorToInt(level.GetNapstablookParalysisTime());
            buff.SetProperty(SwordParalyzedBuff.PROP_TIMEOUT, timeout);
            level.PlaySound(VanillaSoundID.shock);
        }
        public static bool IsParalyzed(LevelEngine level)
        {
            return level.HasBuff<SwordParalyzedBuff>();
        }
        private void Swing(LevelEngine level)
        {
            if (IsParalyzed(level))
                return;
            level.GetHeldItemModelInterface()?.TriggerAnimation("Swing");
            level.PlaySound(VanillaSoundID.swing);
        }
    }
}
