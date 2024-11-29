using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.glowstone)]
    [EntitySeedDefinition(25, VanillaMod.spaceName, VanillaRechargeNames.longTime)]
    public class Glowstone : ContraptionBehaviour
    {
        public Glowstone(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.PlaySound(VanillaSoundID.glowstone);
            entity.UpdateShineRing();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.UpdateShineRing();
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.AddBuff<GlowstoneEvokeBuff>();
            entity.Level.Spawn(VanillaEffectID.stunningFlash, entity.GetBoundsCenter(), entity);
            bool stunned = false;
            foreach (var enemy in entity.Level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsHostile(entity)))
            {
                enemy.Stun(150);
                stunned = true;
            }
            if (stunned)
            {
                entity.PlaySound(VanillaSoundID.stunned);
            }
        }
    }
}
