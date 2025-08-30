using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Level;

namespace PVZEngine.Entities
{
    public class ArtifactSourceReference : ILevelSourceReference
    {
        public ArtifactSourceReference()
        {

        }
        public ArtifactSourceReference(Artifact artifact)
        {
            if (artifact == null)
                return;
            id = artifact.Level.GetArtifactIndex(artifact);
            definitionID = artifact.Definition.GetID();
            faction = artifact.Level.Option.LeftFaction;
        }
        public ArtifactSourceReference Clone()
        {
            return new ArtifactSourceReference
            {
                id = ID,
                definitionID = DefinitionID,
                faction = faction,
            };
        }
        public Entity GetEntity(LevelEngine game)
        {
            return game.FindEntityByID(ID);
        }
        public override bool Equals(object obj)
        {
            if (obj is ArtifactSourceReference entityRef)
            {
                return ID == entityRef.ID;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public static bool operator ==(ArtifactSourceReference lhs, ArtifactSourceReference rhs)
        {
            if (lhs is null)
                return rhs is null;
            return lhs.Equals(rhs);
        }
        public static bool operator !=(ArtifactSourceReference lhs, ArtifactSourceReference rhs)
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
        public int Faction => faction;
        public NamespaceID DefinitionID => definitionID;
        public long ID => id;
        ILevelSourceReference ILevelSourceReference.Parent => null;
        private NamespaceID definitionID;
        private int faction = -1;
        private long id;
    }
}