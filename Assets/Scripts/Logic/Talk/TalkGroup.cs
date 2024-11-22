using System.Collections.Generic;
using PVZEngine;

namespace MVZ2Logic.Talk
{
    public class TalkGroup
    {
        public string name;
        public NamespaceID requires;
        public NamespaceID requiresNot;
        public SpriteReference archiveBackground;
        public NamespaceID music;
        public List<NamespaceID> tags;
        public List<TalkSection> sections;
    }
}
