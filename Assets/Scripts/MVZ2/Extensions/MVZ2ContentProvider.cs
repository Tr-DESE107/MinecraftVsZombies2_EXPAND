using MVZ2.Definitions;
using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
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
