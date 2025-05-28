using MVZ2Logic.Artifacts;
using MVZ2Logic.HeldItems;
using MVZ2Logic.IZombie;
using MVZ2Logic.Notes;
using MVZ2Logic.SeedPacks;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public static class LogicGameDefinitionsExt
    {
        public static HeldItemDefinition GetHeldItemDefinition(this IGameContent provider, NamespaceID heldType)
        {
            return provider.GetDefinition<HeldItemDefinition>(LogicDefinitionTypes.HELD_ITEM, heldType);
        }
        public static ArtifactDefinition GetArtifactDefinition(this IGameContent provider, NamespaceID defRef)
        {
            return provider.GetDefinition<ArtifactDefinition>(LogicDefinitionTypes.ARTIFACT, defRef);
        }
        public static SeedOptionDefinition GetSeedOptionDefinition(this IGameContent provider, NamespaceID id)
        {
            return provider.GetDefinition<SeedOptionDefinition>(LogicDefinitionTypes.SEED_OPTION, id);
        }
        public static NoteDefinition GetNoteDefinition(this IGameContent provider, NamespaceID heldType)
        {
            return provider.GetDefinition<NoteDefinition>(LogicDefinitionTypes.NOTE, heldType);
        }
        public static IZombieLayoutDefinition GetIZombieLayoutDefinition(this IGameContent provider, NamespaceID id)
        {
            return provider.GetDefinition<IZombieLayoutDefinition>(LogicDefinitionTypes.I_ZOMBIE_LAYOUT, id);
        }
    }
}
