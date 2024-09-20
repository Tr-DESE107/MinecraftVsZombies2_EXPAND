using PVZEngine.Definitions;

namespace PVZEngine.Game
{
    public static class ContentProviderHelper
    {
        public static TalkEndDefinition GetTalkEndDefinition(this IContentProvider provider, NamespaceID defRef)
        {
            return provider.GetDefinition<TalkEndDefinition>(defRef);
        }
    }
}
