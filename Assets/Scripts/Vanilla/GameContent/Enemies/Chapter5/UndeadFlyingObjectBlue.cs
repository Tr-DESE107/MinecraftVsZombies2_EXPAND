using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.undeadFlyingObjectBlue)]
    public class UndeadFlyingObjectBlue : UndeadFlyingObject
    {
        public UndeadFlyingObjectBlue(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.State == STATE_ACT)
            {
                if (!entity.HasBuff<UFOBlueAbsorbBuff>())
                {
                    entity.AddBuff<UFOBlueAbsorbBuff>();
                }
            }
            else
            {
                if (entity.HasBuff<UFOBlueAbsorbBuff>())
                {
                    entity.RemoveBuffs<UFOBlueAbsorbBuff>();
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var absorbedEntities = GetAbsorbedEntityID(entity);
            if (absorbedEntities != null)
            {
                foreach (var stolen in absorbedEntities)
                {
                    if (NamespaceID.IsValid(stolen))
                    {
                        entity.Spawn(stolen, entity.GetCenter());
                    }
                }
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
                SetUFOState(enemy, STATE_ACT);
                timer.ResetTime(GetActTime());
            }
        }
        protected override void UpdateStateAct(Entity enemy)
        {
            base.UpdateStateAct(enemy);
            var timer = GetStateTimer(enemy);
            timer.Run();
            if (timer.Expired)
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
        public static List<NamespaceID> GetAbsorbedEntityID(Entity entity) => entity.GetBehaviourField<List<NamespaceID>>(PROP_ABSORBED_ENTITY_ID);
        public static void SetAbsorbedEntityID(Entity entity, List<NamespaceID> value) => entity.SetBehaviourField(PROP_ABSORBED_ENTITY_ID, value);
        public static void AddAbsorbedEntityID(Entity entity, NamespaceID value)
        {
            var list = GetAbsorbedEntityID(entity);
            if (list == null)
            {
                list = new List<NamespaceID>();
                SetAbsorbedEntityID(entity, list);
            }
            list.Add(value);
        }
        public static bool RemoveAbsorbedEntityID(Entity entity, NamespaceID value)
        {
            var list = GetAbsorbedEntityID(entity);
            if (list == null)
            {
                return false;
            }
            return list.Remove(value);
        }

        public const int STAY_TIME = 30;
        public const int ACT_TIME = 300;
        public const int STEAL_CONTRAPTION_TIME = 30;
        public static readonly VanillaEntityPropertyMeta<List<NamespaceID>> PROP_ABSORBED_ENTITY_ID = new VanillaEntityPropertyMeta<List<NamespaceID>>("absorbed_entity_id");
    }
}
