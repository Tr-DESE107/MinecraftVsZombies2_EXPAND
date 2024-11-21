using MVZ2Logic.HeldItems;
using MVZ2Logic.Notes;
using PVZEngine;

namespace MVZ2Logic.Callbacks
{
    public static class MVZ2ContentProvider
    {
        public static HeldItemDefinition GetHeldItemDefinition(this IContentProvider provider, NamespaceID heldType)
        {
            return provider.GetDefinition<HeldItemDefinition>(heldType);
        }
        public static NoteDefinition GetNoteDefinition(this IContentProvider provider, NamespaceID heldType)
        {
            return provider.GetDefinition<NoteDefinition>(heldType);
        }
    }
}
