using System.Collections.Generic;
using PVZEngine;

namespace MVZ2.TalkData
{
    public class TalkSentence
    {
        public string text;
        public NamespaceID speaker;
        public NamespaceID descriptionId;
        public List<NamespaceID> sounds;
        public string variant;
        public List<TalkScript> startScripts;
        public List<TalkScript> clickScripts;
    }
}
