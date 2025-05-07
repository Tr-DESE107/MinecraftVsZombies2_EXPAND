using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Obstacles;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.devourer)]
    public class Devourer : ContraptionBehaviour
    {
        public Devourer(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity devourer)
        {
            base.Init(devourer);
            devourer.Timeout = devourer.GetMaxTimeout();
            devourer.Level.AddLoopSoundEntity(VanillaSoundID.metalSaw, devourer.ID);
            devourer.Target = GetMillTarget(devourer);
            if (!devourer.Target.ExistsAndAlive())
            {
                devourer.Die(new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), devourer);
            }
            UpdateDevourerPosition(devourer);
            devourer.SetAnimationBool("Mill", true);
        }
        protected override void UpdateLogic(Entity devourer)
        {
            base.UpdateLogic(devourer);
            if (!devourer.Target.ExistsAndAlive())
            {
                devourer.Die(new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), devourer);
                return;
            }
            UpdateDevourerPosition(devourer);
            devourer.Timeout--;
            if (devourer.Timeout <= 0)
            {
                var target = devourer.Target;
                if (devourer.Target.IsEntityOf(VanillaObstacleID.monsterSpawner))
                {
                    target.Remove();
                    devourer.Produce(VanillaPickupID.emerald);
                }
                else
                {
                    target.Remove();
                    var pickup = devourer.Produce(VanillaPickupID.blueprintPickup);
                    var entityID = target.GetDefinitionID();
                    var blueprintID = entityID;
                    BlueprintPickup.SetBlueprintID(pickup, blueprintID);
                }
                devourer.Remove();
            }
        }
        public static Entity GetMillTarget(Entity devourer)
        {
            var grid = devourer.GetGrid();
            if (grid == null)
                return null;

            var gridEntities = grid.GetEntities();
            var spawner = gridEntities.FirstOrDefault(e => e.IsEntityOf(VanillaObstacleID.monsterSpawner));
            if (spawner.ExistsAndAlive())
                return spawner;

            var layers = grid.GetLayers();
            var orderedLayers = layers.OrderBy(l => Global.Game.GetGridLayerGroup(l)).ThenByDescending(l => Global.Game.GetGridLayerPriority(l));
            foreach (var layer in orderedLayers)
            {
                var entity = grid.GetLayerEntity(layer);
                if (entity == devourer)
                    continue;
                if (!CanMill(entity))
                    continue;
                return entity;
            }
            return null;
        }
        public static bool CanMill(Entity entity)
        {
            if (!entity.ExistsAndAlive())
                return false;
            if (!entity.IsEntityOf(VanillaObstacleID.monsterSpawner) && entity.Type != EntityTypes.PLANT)
                return false;
            return true;
        }
        private void UpdateDevourerPosition(Entity devourer)
        {
            var pos = devourer.Target.Position;
            pos.y += devourer.Timeout / (float)devourer.GetMaxTimeout() * DEVOUR_START_HEIGHT;
            pos.z -= 0.01f;
            devourer.Position = pos;
        }
        public const float DEVOUR_START_HEIGHT = 48f;
    }
}
