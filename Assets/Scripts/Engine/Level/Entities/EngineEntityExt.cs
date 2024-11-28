using PVZEngine.Damages;

namespace PVZEngine.Entities
{
    public static class EngineEntityExt
    {
        public static void SetBehaviourProperty(this Entity entity, NamespaceID id, string name, object value)
        {
            entity.SetProperty($"{id}/{name}", value);
        }
        public static T GetBehaviourProperty<T>(this Entity entity, NamespaceID id, string name)
        {
            return entity.GetProperty<T>($"{id}/{name}");
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
    }
}
