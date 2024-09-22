using MVZ2.Definitions;
using PVZEngine;

namespace MVZ2.Games
{
    public static class ContentProviderHelper
    {
        public static TalkEndDefinition GetTalkEndDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<TalkEndDefinition>(defRef);
        }
    }
}
