using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Talk
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
