using System.Collections.Generic;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    public class UFOBehaviourBlue : UFOBehaviour
    {
        public UFOBehaviourBlue() : base(UndeadFlyingObject.VARIANT_BLUE)
        {
        }
        public override bool CanSpawn(LevelEngine level)
        {
            return true;
        }
        public override void UpdateLogic(Entity entity)
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
        }
        public override void UpdateActionState(Entity entity, int state)
        {
            base.UpdateActionState(entity, state);
            switch (state)
            {
                case STATE_STAY:
                    UpdateStateStay(entity);
                    break;
                case STATE_ACT:
                    UpdateStateAct(entity);
                    break;
                case STATE_LEAVE:
                    UpdateStateLeave(entity);
                    break;
            }
        }
        private void UpdateStateStay(Entity enemy)
        {
            EnterUpdate(enemy);

            var timer = GetOrInitStateTimer(enemy, STAY_TIME);
            timer.Run();
            if (timer.Expired)
            {
                SetUFOState(enemy, STATE_ACT);
                timer.ResetTime(ACT_TIME);
            }
        }
        private void UpdateStateAct(Entity enemy)
        {
            EnterUpdate(enemy);

            var timer = GetOrInitStateTimer(enemy, ACT_TIME);
            timer.Run();
            if (timer.Expired)
            {
                SetUFOState(enemy, STATE_LEAVE);
            }
        }
        private void UpdateStateLeave(Entity entity)
        {
            LeaveUpdate(entity);
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
        public const string PROP_REGION = VanillaEnemyNames.ufo;
        [PropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta<List<NamespaceID>> PROP_ABSORBED_ENTITY_ID = new VanillaEntityPropertyMeta<List<NamespaceID>>("absorbed_entity_id");
    }
}
