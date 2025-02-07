using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.stoneShield)]
    public class StoneShield : ContraptionBehaviour
    {
        public StoneShield(string nsp, string name) : base(nsp, name)
        {
            AddAura(new ProtectExplosionAura());
            evocationDetector = new SphereDetector(100)
            {
                includeSelf = true,
                factionTarget = FactionTarget.Friendly
            };
        }
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);
            contraption.SetAnimationInt("HealthState", contraption.GetHealthState(3));
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
            detectBuffer.Clear();
            evocationDetector.DetectEntities(contraption, detectBuffer);
            foreach (var ent in detectBuffer)
            {
                if (ent.Type == EntityTypes.PLANT)
                {
                    var buff = ent.GetFirstBuff<IronCurtainBuff>();
                    if (buff == null)
                    {
                        buff = ent.AddBuff<IronCurtainBuff>();
                    }
                    buff.SetProperty(IronCurtainBuff.PROP_TIMEOUT, IronCurtainBuff.MAX_TIMEOUT);
                }
            }
            contraption.PlaySound(VanillaSoundID.ironCurtain);
        }
        public class ProtectExplosionAura : AuraEffectDefinition
        {
            public ProtectExplosionAura()
            {
                BuffID = VanillaBuffID.stoneShieldProtected;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var source = auraEffect.Source;
                var sourceEnt = source.GetEntity();
                if (sourceEnt == null)
                    return;
                var grid = sourceEnt.GetGrid();
                if (grid == null)
                    return;
                var main = grid.GetMainEntity();
                if (main != null)
                {
                    results.Add(main);
                }
                var carrier = grid.GetCarrierEntity();
                if (carrier != null)
                {
                    results.Add(carrier);
                }
            }
        }
        private Detector evocationDetector;
        private List<Entity> detectBuffer = new List<Entity>();
    }
}
