﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.sword)]
    public class SwordHeldItemBehaviour : HeldItemBehaviourDefinition
    {
        public SwordHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void GetModelID(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            result.SetFinalValue(VanillaModelID.swordHeldItem);
        }
        public override void GetRadius(LevelEngine level, IHeldItemData data, CallbackResult result)
        {
            result.SetFinalValue(16);
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
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
                                return entity.IsHostileEntity() || entity.IsEntityOf(VanillaEnemyID.napstablook);
                        }
                        return false;
                    }
            }

            return false;
        }
        public override void OnUpdate(LevelEngine level, IHeldItemData data)
        {
            base.OnUpdate(level, data);
            level.GetHeldItemModelInterface()?.SetAnimationBool("Paralyzed", level.HasBuff<SwordParalyzedBuff>());
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            base.OnPointerEvent(target, data, pointerParams);
            if (pointerParams.IsInvalidClickButton())
                return;
            OnMainPointerEvent(target, data, pointerParams);
            PointerDown(target, data, pointerParams);
        }
        private void OnMainPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            switch (target)
            {
                case HeldItemTargetEntity entityTarget:
                    OnPointerEventEntity(entityTarget, data, pointerParams);
                    break;
            }
        }
        private void OnPointerEventEntity(HeldItemTargetEntity target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var entity = target.Target;

            switch (entity.Type)
            {
                case EntityTypes.ENEMY:
                    if (pointerParams.interaction == PointerInteraction.Down)
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
        private void PointerDown(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (pointerParams.interaction == PointerInteraction.Down)
            {
                Swing(target.GetLevel());
            }
        }
        public static void Paralyze(LevelEngine level)
        {
            var buff = level.AddBuff<SwordParalyzedBuff>();
            var timeout = level.GetNapstablookParalysisTime();
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
