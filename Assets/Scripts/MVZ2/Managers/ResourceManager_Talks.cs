using System.Linq;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        public TalkGroup GetTalkGroup(NamespaceID groupID)
        {
            if (!NamespaceID.IsValid(groupID))
                return null;
            var modResource = GetModResource(groupID.spacename);
            if (modResource == null)
                return null;
            foreach (var meta in modResource.TalkMetas.Values)
            {
                var group = meta.groups.FirstOrDefault(g => g.name == groupID.path);
                if (group != null)
                    return group;
            }
            return null;
        }
        public TalkSection GetTalkSection(NamespaceID groupID, int sectionIndex)
        {
            var group = GetTalkGroup(groupID);
            if (group?.sections == null)
                return null;
            if (sectionIndex < 0 || sectionIndex >= group.sections.Count)
                return null;
            return group.sections[sectionIndex];
        }
        public TalkSentence GetTalkSentence(NamespaceID groupID, int sectionIndex, int sentenceIndex)
        {
            var section = GetTalkSection(groupID, sectionIndex);
            if (section?.sentences == null)
                return null;
            if (sentenceIndex < 0 || sentenceIndex >= section.sentences.Count)
                return null;
            return section.sentences[sentenceIndex];
        }
    }
}
