using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using static MVZ2.GameContent.Buffs.VanillaBuffNames;
using Tools;
using MVZ2Logic;
using System.Linq;
using MVZ2.Vanilla.Grids;

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
            var state = contraption.GetHealthState(3);
            contraption.SetAnimationInt("HealthState", state);
        }

        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            var grid = entity.GetGrid();
            if (grid == null)
                return;

            var game = Global.Game;
            var level = entity.Level;
            var rng = entity.RNG;
            entity.ClearTakenGrids();
            var unlockedContraptions = game.GetUnlockedContraptions();
            var validContraptions = unlockedContraptions.Where(id => game.IsContraptionInAlmanac(id) && grid.GetEntityPlaceStatus(id) == null);
            if (validContraptions.Count() <= 0)
                return;
            var contraptionID = validContraptions.Random(rng);
            var spawnParam = entity.GetSpawnParams();
            var spawned = entity.Spawn(contraptionID, entity.Position, spawnParam);
            if (spawned != null && spawned.HasBuff<NocturnalBuff>())
            {
                spawned.RemoveBuffs<NocturnalBuff>();
            }
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
        }
    }
}
