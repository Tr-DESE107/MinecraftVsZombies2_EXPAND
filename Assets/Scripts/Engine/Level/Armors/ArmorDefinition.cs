using System.Collections.Generic;
using System.Linq;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level.Collisions;

namespace PVZEngine.Armors
{
    public class ArmorDefinition : Definition, ICachedDefinition
    {
        public ArmorDefinition(string nsp, string name, ColliderConstructor[] constructors) : base(nsp, name)
        {
            colliderConstructors = constructors;
        }
        public void AddBehaviour(NamespaceID behaviour)
        {
            behaviours.Add(behaviour);
        }
        public bool RemoveBehaviour(NamespaceID behaviour)
        {
            return behaviours.Remove(behaviour);
        }
        public bool HasBehaviour(NamespaceID behaviour)
        {
            return behaviours.Contains(behaviour);
        }
        public bool HasBehaviour(ArmorBehaviourDefinition behaviour)
        {
            return HasBehaviour(behaviour.GetID());
        }
        public void PostUpdate(Armor armor)
        {
            foreach (var behaviour in behaviourCaches)
            {
                behaviour.PostUpdate(armor);
            }
        }
        void ICachedDefinition.CacheContents(IGameContent content)
        {
            behaviourCaches.Clear();
            foreach (var behaviourID in behaviours)
            {
                var behaviour = content.GetArmorBehaviourDefinition(behaviourID);
                if (behaviour == null)
                    continue;
                behaviourCaches.Add(behaviour);
            }
        }
        void ICachedDefinition.ClearCaches()
        {
            behaviourCaches.Clear();
        }
        public NamespaceID GetModelID()
        {
            return GetID().ToModelID(EngineModelID.TYPE_ARMOR);
        }
        public AuraEffectDefinition[] GetAuras()
        {
            return behaviourCaches.SelectMany(b => b.GetAuras()).ToArray();
        }
        public virtual IEnumerable<ColliderConstructor> GetColliderConstructors(Entity entity, NamespaceID slotID)
        {
            return colliderConstructors;
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ARMOR;
        protected ColliderConstructor[] colliderConstructors;
        private List<NamespaceID> behaviours = new List<NamespaceID>();
        private List<ArmorBehaviourDefinition> behaviourCaches = new List<ArmorBehaviourDefinition>();
    }
}
