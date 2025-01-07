using MVZ2.GameContent.Talk;
using MVZ2.Vanilla;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.GameContent.Maps
{
    public static class VanillaArchiveBackgrounds
    {
        public static readonly SpriteReference nightmare = Get("misc/nightmare");
        private static SpriteReference Get(string name)
        {
            return new SpriteReference(new NamespaceID(VanillaMod.spaceName, name));
        }
    }
}
