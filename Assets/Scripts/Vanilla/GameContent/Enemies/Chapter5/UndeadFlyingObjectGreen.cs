using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
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
            entity.Remove();
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
                    if (!CanStartSteal(entity))
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
        public static bool CanStartSteal(Entity entity)
        {
            return entity.ExistsAndAlive() && !entity.HasBuff<StolenByUFOBuff>();
        }
        public static NamespaceID GetStolenEntityID(Entity entity) => entity.GetBehaviourField<NamespaceID>(PROP_STOLEN_ENTITY_ID);
        public static void SetStolenEntityID(Entity entity, NamespaceID value) => entity.SetBehaviourField(PROP_STOLEN_ENTITY_ID, value);

        public const int STAY_TIME = 240;
        public const int ACT_TIME = 150;
        public const int STEAL_CONTRAPTION_TIME = 30;
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_STOLEN_ENTITY_ID = new VanillaEntityPropertyMeta<NamespaceID>("stolen_entity_id");
    }
}
