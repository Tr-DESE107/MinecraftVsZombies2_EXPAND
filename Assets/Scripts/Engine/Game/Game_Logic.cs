using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;

namespace PVZEngine.Game
{
    public partial class Game
    {
        public TalkEndDefinition GetTalkEndDefinition(NamespaceID defRef)
        {
            return talkEndDefintiions.FirstOrDefault(d => d.GetID() == defRef);
        }
        private List<TalkEndDefinition> talkEndDefintiions = new List<TalkEndDefinition>();
    }
}