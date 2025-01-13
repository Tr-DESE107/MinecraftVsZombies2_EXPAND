using PVZEngine.Damages;
using UnityEngine;

namespace PVZEngine.Entities
{
    public static class EngineEntityExt
    {
        public static void SetBehaviourField(this Entity entity, NamespaceID id, string name, object value)
        {
            entity.SetField(id.ToString(), name, value);
        }
        public static T GetBehaviourField<T>(this Entity entity, NamespaceID id, string name)
        {
            return entity.GetField<T>(id.ToString(), name);
        }
        public static ShellDefinition GetShellDefinition(this Entity entity)
        {
            var shellID = entity.GetShellID();
            return entity.Level.Content.GetShellDefinition(shellID);
        }
        public static NamespaceID GetDefinitionID(this Entity entity)
        {
            return entity?.Definition?.GetID();
        }
        public static Entity Spawn(this Entity entity, NamespaceID id, Vector3 position)
        {
            return entity.Level.Spawn(id, position, entity);
        }
    }
}
