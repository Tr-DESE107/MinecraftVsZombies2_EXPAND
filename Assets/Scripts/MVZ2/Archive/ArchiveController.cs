using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Archives;
using MVZ2Logic.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Archives
{
    public class ArchiveController : MainScenePage, IArchiveInterface
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(ArchiveUI.Page.Index);
            UpdateIndex();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnSearchEndEdit += OnSearchEndEditCallback;
            ui.OnTalkTagValueChanged += OnTalkTagValueChangedCallback;
            ui.OnTalkEntryClick += OnTalkEntryClickCallback;

            ui.OnDetailsReturnClick += OnDetailsReturnClickCallback;
            ui.OnDetailsPlayClick += OnDetailsPlayClickCallback;

            talkSystem = new ArchiveTalkSystem(this, simulationTalk);
            simulationTalk.OnTalkAction += OnTalkActionCallback;
        }
        #endregion

        #region 事件回调
        private void OnIndexReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnDetailsReturnClickCallback()
        {
            ui.DisplayPage(ArchiveUI.Page.Index);
        }
        private void OnSearchEndEditCallback(string value)
        {
            searchPattern = value;
            UpdateFilteredTalks();
        }
        private void OnTalkTagValueChangedCallback(int index, bool value)
        {
            if (value)
            {
                selectedTagIndexes.Add(index);
            }
            else
            {
                selectedTagIndexes.Remove(index);
            }
            UpdateFilteredTalks();
        }
        private void OnTalkEntryClickCallback(int index)
        {
            var groupID = filteredTalks[index];
            viewingTalkID = groupID;
            UpdateDetails(groupID);
            ui.DisplayPage(ArchiveUI.Page.Details);
        }
        private void OnDetailsPlayClickCallback()
        {
            PlayTalk(viewingTalkID);
        }
        private void OnTalkActionCallback(string cmd, params string[] parameters)
        {
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, new VanillaCallbacks.TalkActionParams(talkSystem, cmd, parameters), cmd);
        }
        #endregion

        void IArchiveInterface.SetBackground(SpriteReference backgroundRef)
        {
            var background = Main.GetFinalSprite(backgroundRef);
            ui.SetSimulationBackground(background);
        }
        private void ShowReplayDialog()
        {
            var title = Main.LanguageManager._p(VanillaStrings.CONTEXT_ARCHIVE, VanillaStrings.ARCHIVE_TALK_END);
            var desc = Main.LanguageManager._p(VanillaStrings.CONTEXT_ARCHIVE, VanillaStrings.ARCHIVE_REPLAY);
            Main.Scene.ShowDialogSelect(title, desc, (value) =>
            {
                if (value)
                {
                    PlayTalk(viewingTalkID);
                }
                else
                {
                    ReturnFromSimulation();
                }
            });
        }
        private void UpdateIndex()
        {
            searchPattern = string.Empty;
            ui.SetIndexSearch(searchPattern);

            talksList.Clear();
            var talks = Main.ResourceManager.GetAllTalkGroupsID();
            var talkGroups = talks
                .Select(id => (id, Main.ResourceManager.GetTalkGroup(id)))
                .OrderBy(g => g.Item2.documentOrder)
                .ThenBy(g => g.Item2.groupOrder);

            var filteredTalks = talkGroups.Where(tuple =>
            {
                var unlockID = tuple.Item2?.archive?.unlock;
                return Main.SaveManager.IsInvalidOrUnlocked(unlockID);
            });
            talksList.AddRange(filteredTalks.Select(g => g.Item1));

            tagsList.Clear();
            var tags = talkGroups.SelectMany(g => g.Item2.tags).Distinct();
            var orderedTags = tags.OrderBy((t) => Main.ResourceManager.GetArchiveTagMeta(t)?.Priority ?? 0);
            tagsList.AddRange(orderedTags);

            selectedTagIndexes.Clear();
            var tagViewDatas = tagsList.Select((tag, index) => new ArchiveTagViewData() { name = Main.ResourceManager.GetArchiveTagName(tag), value = selectedTagIndexes.Contains(index) }).ToArray();
            ui.SetIndexTags(tagViewDatas);

            UpdateFilteredTalks();
        }
        private void UpdateDetails(NamespaceID groupID)
        {
            var group = Main.ResourceManager.GetTalkGroup(groupID);
            if (group == null)
                return;
            var name = GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, group.archive.name);
            var backgroundRef = group.archive.background;
            var background = Main.GetFinalSprite(backgroundRef);
            var musicID = group.archive.music;
            var music = Main.ResourceManager.GetMusicName(musicID);
            var tags = string.Join(", ", group.tags.Select(t => Main.ResourceManager.GetArchiveTagName(t)));
            var sections = group.sections;
            var sectionsViewData = new ArchiveDetailsSectionViewData[sections.Count];
            for (int i = 0; i < sectionsViewData.Length; i++)
            {
                var section = sections[i];
                var description = GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, section.archiveText);
                var talks = section.sentences.Select(s =>
                {
                    var description = GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, s.description);
                    string characterName = s.GetSpeakerName(Main);
                    var text = GetTranslatedString(VanillaStrings.GetTalkTextContext(groupID), s.text);
                    if (string.IsNullOrEmpty(description))
                    {
                        return GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, SENTENCE_TEMPLATE, characterName, text);
                    }
                    else
                    {
                        return GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, SENTENCE_TEMPLATE_DESCRIPTION, description, characterName, text);
                    }
                });
                var talksString = string.Join('\n', talks);
                sectionsViewData[i] = new ArchiveDetailsSectionViewData()
                {
                    description = description,
                    talks = talksString
                };
            }
            var viewData = new ArchiveDetailsViewData()
            {
                name = name,
                background = background,
                segments = sections.Count.ToString(),
                music = music,
                tags = tags,
                sections = sectionsViewData
            };
            ui.UpdateDetails(viewData);
        }
        private void UpdateFilteredTalks()
        {
            filteredTalks.Clear();
            var talkGroups = talksList.Where(t =>
            {
                var group = Main.ResourceManager.GetTalkGroup(t);
                var name = GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, group.archive.name);
                var selectedTags = selectedTagIndexes.Select(i => tagsList[i]);
                return (string.IsNullOrEmpty(searchPattern) || name.Contains(searchPattern)) && (selectedTags.Count() <= 0 || selectedTags.Any(t => group.tags.Contains(t)));
            });
            filteredTalks.AddRange(talkGroups);
            ui.SetIndexTalks(filteredTalks.Select(t =>
            {
                var group = Main.ResourceManager.GetTalkGroup(t);
                return GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, group.archive.name);
            }).ToArray());
        }
        private void PlayTalk(NamespaceID groupID)
        {
            var group = Main.ResourceManager.GetTalkGroup(groupID);
            if (group == null)
                return;
            var backgroundRef = group.archive.background;
            var background = Main.GetFinalSprite(backgroundRef);
            var musicID = group.archive.music;
            ui.SetSimulationBackground(background);
            ui.DisplayPage(ArchiveUI.Page.Simulation);
            simulationTalk.StartTalk(viewingTalkID, 0, onEnd: ShowReplayDialog);
            Main.MusicManager.StopFade();
            Main.MusicManager.SetVolume(1);
            if (NamespaceID.IsValid(musicID))
            {
                Main.MusicManager.Play(musicID);
            }
            else
            {
                Main.MusicManager.Stop();
            }
        }
        private void ReturnFromSimulation()
        {
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
            Main.MusicManager.StopFade();
            Main.MusicManager.SetVolume(1);
            ui.DisplayPage(ArchiveUI.Page.Details);
        }
        private string GetTranslatedString(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
        #endregion
        public event Action OnReturnClick;
        [TranslateMsg("对话档案中语句的模板，{0}为人物，{1}为语句内容")]
        public const string SENTENCE_TEMPLATE = "<b>[{0}]</b> {1}";
        [TranslateMsg("对话档案中语句的模板，{0}为前缀描述，{1}为人物，{2}为语句内容")]
        public const string SENTENCE_TEMPLATE_DESCRIPTION = "<color=blue>{0}</color>\n<b>[{1}]</b> {2}";
        private MainManager Main => MainManager.Instance;
        private string searchPattern;
        private List<int> selectedTagIndexes = new List<int>();
        private List<NamespaceID> tagsList = new List<NamespaceID>();
        private List<NamespaceID> talksList = new List<NamespaceID>();
        private List<NamespaceID> filteredTalks = new List<NamespaceID>();

        private NamespaceID viewingTalkID;

        private ITalkSystem talkSystem;

        [SerializeField]
        private ArchiveUI ui;
        [SerializeField]
        private TalkController simulationTalk;
    }
}
