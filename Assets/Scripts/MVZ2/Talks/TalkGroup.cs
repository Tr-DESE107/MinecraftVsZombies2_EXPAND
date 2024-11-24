using System.Collections.Generic;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.TalkData
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
