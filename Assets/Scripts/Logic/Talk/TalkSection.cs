using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Talk
{
    public class TalkSection
    {
        public NamespaceID nameId;
        public List<TalkScript> startScripts;
        public List<TalkScript> skipScripts;
        public List<TalkCharacter> characters;
        public List<TalkSentence> sentences;
    }
}
