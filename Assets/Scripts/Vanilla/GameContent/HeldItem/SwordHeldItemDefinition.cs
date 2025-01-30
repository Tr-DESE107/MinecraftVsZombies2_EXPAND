using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.sword)]
    public class SwordHeldItemDefinition : HeldItemDefinition
    {
        public SwordHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(LevelEngine level, IHeldItemData data)
        {
            level.GetHeldItemModelInterface()?.SetAnimationBool("Paralyzed", level.HasBuff<SwordParalyzedBuff>());
        }
        public override bool CheckRaycast(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return false;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.ENEMY:
                    return entity.IsHostileEntity();
                case EntityTypes.PICKUP:
                    return !entity.IsCollected();
                case EntityTypes.CART:
                    return !entity.IsCartTriggered();
            }
            return false;
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return HeldHighlight.None;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.CART:
                    return HeldHighlight.Entity;
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction phase)
        {
            switch (target)
            {
                case HeldItemTargetLawn lawnTarget:
                    if (phase == PointerInteraction.Press)
                    {
                        Swing(lawnTarget.Level);
                    }
                    break;
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
                                    Swing(entity.Level);
                                }
                                break;
                            case EntityTypes.PICKUP:
                                if (phase != PointerInteraction.Release)
                                {
                                    if (!entity.IsCollected())
                                        entity.Collect();
                                }
                                break;
                            case EntityTypes.CART:
                                if (phase == PointerInteraction.Press)
                                {
                                    if (!entity.IsCartTriggered())
                                    {
                                        entity.ChargeUpCartTrigger();
                                    }
                                }
                                break;
                        }
                        if (phase == PointerInteraction.Press)
                        {
                            Swing(entity.Level);
                        }
                    }
                    break;
            }
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            return VanillaModelID.swordHeldItem;
        }
        public override float GetRadius(LevelEngine level, IHeldItemData data)
        {
            return 16;
        }
        public static void Paralyze(LevelEngine level)
        {
            var buff = level.AddBuff<SwordParalyzedBuff>();
            var timeout = 45;
            if (level.Difficulty == VanillaDifficulties.easy)
            {
                timeout = 22;
            }
            else if (level.Difficulty == VanillaDifficulties.hard)
            {
                timeout = 90;
            }
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
