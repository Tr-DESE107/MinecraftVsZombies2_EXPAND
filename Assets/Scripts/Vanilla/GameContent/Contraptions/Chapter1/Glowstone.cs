using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.glowstone)]
    public class Glowstone : ContraptionBehaviour
    {
        public Glowstone(string nsp, string name) : base(nsp, name)
        {
            AddAura(new GlowstoneAura());
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
            entity.Level.Spawn(VanillaEffectID.stunningFlash, entity.GetCenter(), entity);
            bool stunned = false;
            foreach (var enemy in entity.Level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsHostile(entity) && e.CanDeactive()))
            {
                enemy.Stun(150);
                stunned = true;
            }
            if (stunned)
            {
                entity.PlaySound(VanillaSoundID.stunned);
            }
        }
        public class GlowstoneAura : AuraEffectDefinition
        {
            public GlowstoneAura()
            {
                BuffID = VanillaBuffID.glowstoneProtected;
                UpdateInterval = 4;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source as Entity;
                if (source == null)
                    return;
                detectBuffer.Clear();
                var level = auraEffect.Level;
                level.GetIlluminatiingEntities(source, detectBuffer);
                foreach (var id in detectBuffer)
                {
                    var ent = level.FindEntityByID(id);
                    if (ent == null)
                        continue;
                    results.Add(ent);
                }
            }
            private HashSet<long> detectBuffer = new HashSet<long>();
        }
    }
}
