using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.lilyPad)]
    public class LilyPad : ContraptionBehaviour
    {
        public LilyPad(string nsp, string name) : base(nsp, name)
        {
            AddAura(new LilyPadPassengerAura());
            AddAura(new LilyPadCarrierAura());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.carriers);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var level = entity.Level;
            var column = entity.GetColumn();
            var lane = entity.GetLane();
            for (int x = column - 2; x <= column + 2; x++)
            {
                for (int y = lane - 2; y <= lane + 2; y++)
                {
                    var grid = level.GetGrid(x, y);
                    if (grid == null || !grid.IsWater() || !grid.IsEmpty())
                        continue;
                    var lily = level.Spawn(VanillaContraptionID.lilyPad, grid.GetEntityPosition(), entity);
                    lily.AddBuff<LilyPadEvocationBuff>();
                    lily.PlaySplashEffect();
                }
            }
            level.PlaySound(VanillaSoundID.water);
        }
        private class LilyPadCarrierAura : AuraEffectDefinition
        {
            public LilyPadCarrierAura()
            {
                BuffID = VanillaBuffID.carryingOther;
            }
            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                var sourceEnt = auraEffect.Source.GetEntity();
                if (sourceEnt != null && sourceEnt.HasPassenger())
                {
                    yield return sourceEnt;
                }
            }
        }
        private class LilyPadPassengerAura : AuraEffectDefinition
        {
            public LilyPadPassengerAura()
            {
                BuffID = VanillaBuffID.carriedByLilyPad;
            }
            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                var sourceEnt = auraEffect.Source.GetEntity();
                var grid = sourceEnt?.GetGrid();
                if (grid != null)
                {
                    yield return grid.GetMainEntity();
                    yield return grid.GetProtectorEntity();
                }
            }
        }
    }
}
