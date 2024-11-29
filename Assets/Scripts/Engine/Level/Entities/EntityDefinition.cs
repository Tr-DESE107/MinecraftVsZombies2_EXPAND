using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Damages;
using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Entities
{
    public abstract class EntityDefinition : Definition
    {
        public EntityDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public void SetBehaviour(EntityBehaviourDefinition behaviour)
        {
            this.behaviour = behaviour;
        }
        public EntityBehaviourDefinition GetBehaviour()
        {
            return behaviour;
        }
        public T GetBehaviour<T>()
        {
            if (behaviour is T tBehaviour)
                return tBehaviour;
            return default;
        }
        public void Init(Entity entity) { behaviour?.Init(entity); }
        public void Update(Entity entity) { behaviour?.Update(entity); }
        public void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult) { behaviour?.PostTakeDamage(bodyResult, armorResult); }
        public void PostContactGround(Entity entity, Vector3 velocity) { behaviour?.PostContactGround(entity, velocity); }
        public void PostLeaveGround(Entity entity) { behaviour?.PostLeaveGround(entity); }
        public void PostCollision(Entity entity, Entity other, int state) { behaviour?.PostCollision(entity, other, state); }
        public void PostDeath(Entity entity, DamageInfo damageInfo) { behaviour?.PostDeath(entity, damageInfo); }
        public void PostRemove(Entity entity) { behaviour?.PostRemove(entity); }
        public void PostEquipArmor(Entity entity, Armor armor) { behaviour?.PostEquipArmor(entity, armor); }
        public void PostDestroyArmor(Entity entity, Armor armor, DamageResult damage) { behaviour?.PostDestroyArmor(entity, armor, damage); }
        public void PostRemoveArmor(Entity entity, Armor armor) { behaviour?.PostRemoveArmor(entity, armor); }
        public NamespaceID GetModelID(LevelEngine level)
        {
            NamespaceID id;
            if (!TryGetProperty<NamespaceID>(EngineEntityProps.MODEL_ID, out id) || !NamespaceID.IsValid(id))
            {
                id = GetID().ToModelID(EngineModelID.TYPE_ENTITY);
            }
            if (behaviour != null)
            {
                id = behaviour.GetModelID(level, id);
            }
            return id;
        }
        public abstract int Type { get; }
        private EntityBehaviourDefinition behaviour;
    }
}
