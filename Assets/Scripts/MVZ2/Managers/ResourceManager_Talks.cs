using System.Collections.Generic;
using System.Linq;
using MVZ2.Metas;
using MVZ2.TalkData;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class ResourceManager : MonoBehaviour
    {
        #region 对话组
        public TalkGroup GetTalkGroup(NamespaceID groupID)
        {
            if (!NamespaceID.IsValid(groupID))
                return null;
            return talksCacheDict.TryGetValue(groupID, out var meta) ? meta : null;
        }
        public bool HasTalkGroup(NamespaceID groupID)
        {
            return GetTalkGroup(groupID) != null;
        }
        public NamespaceID[] GetAllTalkGroupsID()
        {
            return talksCacheDict.Keys.ToArray();
        }
        public bool CanStartTalk(NamespaceID groupId, int sectionIndex)
        {
            var group = GetTalkGroup(groupId);
            if (group == null)
                return false;
            if (Main.SaveManager.IsValidAndLocked(group.requires))
                return false;
            if (Main.SaveManager.IsUnlocked(group.requiresNot))
                return false;
            var section = GetTalkSection(groupId, sectionIndex);
            if (section == null)
                return false;
            return true;
        }
        public bool WillSkipTalk(NamespaceID groupId, int sectionIndex)
        {
            if (!Main.OptionsManager.SkipAllTalks())
                return false;
            var section = GetTalkSection(groupId, sectionIndex);
            if (section == null)
                return false;
            if (!section.canAutoSkip)
                return false;
            return true;
        }
        #endregion

        #region 对话段落
        public TalkSection GetTalkSection(NamespaceID groupID, int sectionIndex)
        {
            var group = GetTalkGroup(groupID);
            if (group?.sections == null)
                return null;
            if (sectionIndex < 0 || sectionIndex >= group.sections.Count)
                return null;
            return group.sections[sectionIndex];
        }
        #endregion

        #region 对话语句
        public TalkSentence GetTalkSentence(NamespaceID groupID, int sectionIndex, int sentenceIndex)
        {
            var section = GetTalkSection(groupID, sectionIndex);
            if (section?.sentences == null)
                return null;
            if (sentenceIndex < 0 || sentenceIndex >= section.sentences.Count)
                return null;
            return section.sentences[sentenceIndex];
        }
        #endregion

        #region 档案标签
        public ArchiveMetaList GetArchiveMetaList(string nsp)
        {
            var modResource = GetModResource(nsp);
            if (modResource == null)
                return null;
            return modResource.ArchiveMetaList;
        }
        public ArchiveTagMeta GetArchiveTagMeta(NamespaceID tagID)
        {
            var metaList = GetArchiveMetaList(tagID.SpaceName);
            if (metaList == null)
                return null;
            return metaList.Tags.FirstOrDefault(t => t.ID == tagID.Path);
        }
        public string GetArchiveTagName(NamespaceID tagID)
        {
            var meta = GetArchiveTagMeta(tagID);
            if (meta == null)
                return string.Empty;
            return main.LanguageManager._p(VanillaStrings.CONTEXT_ARCHIVE_TAG_NAME, meta.Name);
        }
        #endregion

        private Dictionary<NamespaceID, TalkGroup> talksCacheDict = new Dictionary<NamespaceID, TalkGroup>();
    }
}
