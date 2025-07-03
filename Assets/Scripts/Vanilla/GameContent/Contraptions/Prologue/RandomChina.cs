using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.randomChina)]
    public class RandomChina : ContraptionBehaviour
    {
        public RandomChina(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);
            contraption.SetModelDamagePercent();
        }

        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;

            var grid = entity.GetGrid();
            if (grid == null)
                return;

            var game = Global.Game;
            var level = entity.Level;
            var rng = entity.RNG;
            entity.ClearTakenGrids();
            var unlockedContraptions = game.GetUnlockedContraptions();
            var validContraptions = unlockedContraptions.Where(id =>
            {
                if (!game.IsContraptionInAlmanac(id))
                    return false;
                var def = game.GetEntityDefinition(id);
                if (def.IsUpgradeBlueprint())
                    return false;
                return grid.CanSpawnEntity(id);
            });
            if (validContraptions.Count() <= 0)
                return;
            var contraptionID = validContraptions.Random(rng);
            var spawned = entity.SpawnWithParams(contraptionID, entity.Position);
            if (spawned != null && spawned.HasBuff<NocturnalBuff>())
            {
                spawned.RemoveBuffs<NocturnalBuff>();
            }
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
            var rng = new RandomGenerator(contraption.RNG.Next());

            var allEvents = contraption.Level.Content.GetDefinitions<RandomChinaEventDefinition>(VanillaDefinitionTypes.RANDOM_CHINA_EVENT);
            var def = allEvents.Random(rng);
            //var def = contraption.Level.Content.GetDefinition<RandomChinaEventDefinition>(VanillaDefinitionTypes.RANDOM_CHINA_EVENT, VanillaRandomChinaEventID.ancientEgypt);
            def.Run(contraption, rng);
            var nameKey = def.Text;
            contraption.Level.ShowAdvice(VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME, nameKey, 0, 90);
        }
    }
}
