using MVZ2Logic.Artifacts;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Notes;
using PVZEngine;

namespace MVZ2Logic.Callbacks
{
    public static class LogicGameDefinitionsExt
    {
        public static HeldItemDefinition GetHeldItemDefinition(this IGameContent provider, NamespaceID heldType)
        {
            return provider.GetDefinition<HeldItemDefinition>(heldType);
        }
        public static ArtifactDefinition GetArtifactDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArtifactDefinition>(defRef);
        }
        public static NoteDefinition GetNoteDefinition(this IGameContent provider, NamespaceID heldType)
        {
            return provider.GetDefinition<NoteDefinition>(heldType);
        }
    }
}
