using PVZEngine.Level;

namespace PVZEngine.Entities
{
    public class EntityReferenceChain
    {
        public EntityReferenceChain()
        {

        }
        public EntityReferenceChain(Entity entity)
        {
            if (entity == null)
                return;
            id = entity.ID;
            definitionID = entity.Definition.GetID();
            spawnerReference = entity.SpawnerReference?.Clone();
            faction = entity.Cache.Faction;
        }
        public EntityReferenceChain Clone()
        {
            return new EntityReferenceChain
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
            if (obj is EntityReferenceChain entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(EntityReferenceChain lhs, EntityReferenceChain rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(EntityReferenceChain lhs, EntityReferenceChain rhs)
        {
            return !(lhs == rhs);
        }
        public EntityReferenceChain SpawnerReference => spawnerReference;
        public int Faction => faction;
        public NamespaceID DefinitionID => definitionID;
        public long ID => id;
        private NamespaceID definitionID;
        private EntityReferenceChain spawnerReference;
        private int faction = -1;
        private long id;
    }
}