using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.undeadFlyingObjectGreen)]
    public class UndeadFlyingObjectGreen : UndeadFlyingObject
    {
        public UndeadFlyingObjectGreen(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var stolen = GetStolenEntityID(entity);
            if (NamespaceID.IsValid(stolen))
            {
                var blueprintID = VanillaBlueprintID.FromEntity(stolen);
                var spawnParams = entity.GetSpawnParams();
                spawnParams.SetProperty(BlueprintPickup.PROP_BLUEPRINT_ID, blueprintID);
                var pickup = entity.Spawn(VanillaPickupID.blueprintPickup, entity.GetCenter(), spawnParams);
            }
        }
        protected override void UpdateStateStay(Entity enemy)
        {
            base.UpdateStateStay(enemy);
            var timer = GetStateTimer(enemy);
            timer.Run();
            if (timer.Expired)
            {
                var grid = enemy.GetGrid();
                var layers = grid.GetLayers();
                var orderedLayers = VanillaGridLayers.ufoLayers;
                foreach (var layer in orderedLayers)
                {
                    var entity = grid.GetLayerEntity(layer);
                    if (!CanStartSteal(enemy, entity))
                        continue;
                    enemy.Target = entity;
                    var buff = entity.AddBuff<StolenByUFOBuff>();
                    buff.SetProperty(StolenByUFOBuff.PROP_UFO, new EntityID(enemy));
                    break;
                }
                if (!enemy.Target.ExistsAndAlive())
                {
                    SetUFOState(enemy, STATE_LEAVE);
                }
                else
                {
                    SetUFOState(enemy, STATE_ACT);
                }
            }
        }
        protected override void UpdateStateAct(Entity enemy)
        {
            base.UpdateStateAct(enemy);
            if (!enemy.Target.ExistsAndAlive() || !enemy.Target.HasBuff<StolenByUFOBuff>() || NamespaceID.IsValid(GetStolenEntityID(enemy)))
            {
                SetUFOState(enemy, STATE_LEAVE);
            }
        }
        public override int GetStayTime() => STAY_TIME;
        public override int GetActTime() => ACT_TIME;
        public static bool CanStartSteal(Entity ufo, Entity entity)
        {
            return CanStartSteal(ufo.GetFaction(), entity);
        }
        public static bool CanStartSteal(int ufoFaction, Entity entity)
        {
            if (!entity.ExistsAndAlive())
                return false;
            if (entity.Type != EntityTypes.PLANT && entity.Type != EntityTypes.OBSTACLE)
                return false;
            if (entity.HasBuff<StolenByUFOBuff>())
                return false;
            if (!EngineEntityExt.IsHostile(ufoFaction, entity.GetFaction()))
                return false;
            return true;
        }
        public static bool CanSpawn(LevelEngine level)
        {
            return level.GetEntityCount(e => CanStartSteal(level.Option.RightFaction, e)) > 3;
        }
        public static void GetPossibleSpawnGrids(LevelEngine level, HashSet<LawnGrid> results)
        {
            bool filled = false;
            foreach (var ent in level.FindEntities(e => CanStartSteal(level.Option.RightFaction, e)))
            {
                results.Add(ent.GetGrid());
                filled = true;
            }
            if (!filled)
            {
                var maxColumn = level.GetMaxColumnCount();
                var maxLane = level.GetMaxLaneCount();
                for (int x = 0; x < maxColumn; x++)
                {
                    for (int y = 0; y < maxLane; y++)
                    {
                        var grid = level.GetGrid(x, y);
                        if (grid != null)
                        {
                            results.Add(grid);
                        }
                    }
                }
            }
        }
        public static NamespaceID GetStolenEntityID(Entity entity) => entity.GetBehaviourField<NamespaceID>(PROP_STOLEN_ENTITY_ID);
        public static void SetStolenEntityID(Entity entity, NamespaceID value) => entity.SetBehaviourField(PROP_STOLEN_ENTITY_ID, value);

        public const int STAY_TIME = 240;
        public const int ACT_TIME = 150;
        public const int STEAL_CONTRAPTION_TIME = 30;
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_STOLEN_ENTITY_ID = new VanillaEntityPropertyMeta<NamespaceID>("stolen_entity_id");
    }
}
