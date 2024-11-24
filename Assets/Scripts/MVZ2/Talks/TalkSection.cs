using System.Collections.Generic;
using PVZEngine;

namespace MVZ2.TalkData
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
