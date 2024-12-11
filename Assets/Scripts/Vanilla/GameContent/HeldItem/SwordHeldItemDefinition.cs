using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
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
        public override bool IsForEntity() => true;
        public override bool IsForPickup() => true;
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            HeldFlags flags = HeldFlags.None;
            switch (entity.Type)
            {
                case EntityTypes.ENEMY:
                    if (entity.IsHostileEnemy())
                    {
                        flags |= HeldFlags.Valid | HeldFlags.NoHighlight;
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
        private void Swing(LevelEngine level)
        {
            level.GetHeldItemModelInterface()?.TriggerAnimation("Swing");
            level.PlaySound(VanillaSoundID.swing);
        }
    }
}
