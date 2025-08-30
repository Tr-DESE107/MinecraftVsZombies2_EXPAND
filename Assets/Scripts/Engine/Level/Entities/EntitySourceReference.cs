using PVZEngine.Level;

namespace PVZEngine.Entities
{
    public class EntitySourceReference : ILevelSourceReference
    {
        public EntitySourceReference()
        {

        }
        public EntitySourceReference(Entity entity)
        {
            if (entity == null)
                return;
            id = entity.ID;
            definitionID = entity.Definition.GetID();
            spawnerReference = entity.SpawnerReference?.Clone();
            faction = entity.Cache.Faction;
        }
        public EntitySourceReference Clone()
        {
            return new EntitySourceReference
            {
                id = ID,
                definitionID = DefinitionID,
                spawnerReference = SpawnerReference?.Clone(),
                faction = faction,
            };
        }
        public Entity GetEntity(LevelEngine game)
        {
            return game.FindEntityByID(ID);
        }
        public override bool Equals(object obj)
        {
            if (obj is EntitySourceReference entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(EntitySourceReference lhs, EntitySourceReference rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntitySourceReference lhs, EntitySourceReference rhs)
        {
            return !(lhs == rhs);
        }
        ILevelSourceReference ILevelSourceReference.Clone()
        {
            return Clone();
        }
        ILevelSourceTarget ILevelSourceReference.GetTarget(LevelEngine level)
        {
            return GetEntity(level);
        }
        public ILevelSourceReference SpawnerReference => spawnerReference;
        public int Faction => faction;
        public NamespaceID DefinitionID => definitionID;
        public long ID => id;
        ILevelSourceReference ILevelSourceReference.Parent => spawnerReference;
        private NamespaceID definitionID;
        private ILevelSourceReference spawnerReference;
        private int faction = -1;
        private long id;
    }
}