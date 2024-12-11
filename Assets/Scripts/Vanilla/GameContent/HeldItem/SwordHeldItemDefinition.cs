using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.sword)]
    public class SwordHeldItemDefinition : HeldItemDefinition
    {
        public SwordHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(LevelEngine level)
        {
            level.GetHeldItemModelInterface()?.SetAnimationBool("Paralyzed", level.HasBuff<SwordParalyzedBuff>());
        }
        public override bool IsForEntity() => true;
        public override bool IsForPickup() => true;
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            HeldFlags flags = HeldFlags.None;
            switch (entity.Type)
            {
                case EntityTypes.ENEMY:
                    if (!IsParalyzed(entity.Level))
                    {
                        if (entity.IsHostileEnemy())
                        {
                            flags |= HeldFlags.Valid | HeldFlags.NoHighlight;
                        }
                    }
                    break;
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                    {
                        flags |= HeldFlags.Valid;
                    }
                    break;
                case EntityTypes.CART:
                    if (!entity.IsCartTriggered())
                    {
                        flags |= HeldFlags.Valid;
                    }
                    break;
            }
            return flags;
        }
        public override bool UseOnEntity(Entity entity, long id, PointerPhase phase)
        {
            base.UseOnEntity(entity, id, phase);
            switch (entity.Type)
            {
                case EntityTypes.ENEMY:
                    if (phase == PointerPhase.Press)
                    {
                        var effects = new DamageEffectList(VanillaDamageEffects.WHACK, VanillaDamageEffects.REMOVE_ON_DEATH);
                        entity.TakeDamage(750, effects);
                        Swing(entity.Level);
                    }
                    return true;
                case EntityTypes.PICKUP:
                    if (phase != PointerPhase.Release)
                    {
                        if (!entity.IsCollected())
                            entity.Collect();
                        Swing(entity.Level);
                    }
                    return false;
                case EntityTypes.CART:
                    if (phase == PointerPhase.Press)
                    {
                        entity.TriggerCart();
                        Swing(entity.Level);
                    }
                    return false;
            }
            return false;
        }
        public override bool IsForGrid() => false;
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            return HeldFlags.None;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, long id, PointerPhase phase)
        {
            base.UseOnLawn(level, area, id, phase);
            if (phase == PointerPhase.Press)
            {
                Swing(level);
            }
        }
        public override NamespaceID GetModelID(LevelEngine level, long id)
        {
            return VanillaModelID.swordHeldItem;
        }
        public override float GetRadius()
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
