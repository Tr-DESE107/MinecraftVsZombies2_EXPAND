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
using MVZ2Logic;
using MVZ2Logic.Talk;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Archives
{
    public class ArchiveController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            ui.DisplayPage(ArchiveUI.Page.Index);
            UpdateIndex();
            if (!Main.MusicManager.IsPlaying(VanillaMusicID.choosing))
                Main.MusicManager.Play(VanillaMusicID.choosing);
        }
        private void Awake()
        {
            ui.OnIndexReturnClick += OnIndexReturnClickCallback;
            ui.OnSearchEndEdit += OnSearchEndEditCallback;
            ui.OnTalkTagValueChanged += OnTalkTagValueChangedCallback;
            ui.OnTalkEntryClick += OnTalkEntryClickCallback;

            ui.OnDetailsReturnClick += OnDetailsReturnClickCallback;
            ui.OnDetailsPlayClick += OnDetailsPlayClickCallback;

            talkSystem = new ArchiveTalkSystem(simulationTalk);
            simulationTalk.OnTalkAction += OnTalkActionCallback;
        }
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
            Global.Game.RunCallbackFiltered(VanillaCallbacks.TALK_ACTION, cmd, talkSystem, cmd, parameters);
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
            var filteredTalks = talks.Where(t =>
            {
                var group = Main.ResourceManager.GetTalkGroup(t);
                var unlockID = group.archive?.unlock;
                return !NamespaceID.IsValid(unlockID) || Main.SaveManager.IsUnlocked(unlockID);
            });
            talksList.AddRange(filteredTalks);

            var talkGroups = talksList.Select(t => Main.ResourceManager.GetTalkGroup(t));
            tagsList.Clear();
            var tags = talkGroups.SelectMany(g => g.tags).Distinct();
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
            var background = Main.LanguageManager.GetSprite(backgroundRef);
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
                    var character = Main.ResourceManager.GetCharacterName(s.speaker);
                    var text = GetTranslatedString(VanillaStrings.GetTalkTextContext(groupID), s.text);
                    if (string.IsNullOrEmpty(description))
                    {
                        return GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, SENTENCE_TEMPLATE, character, text);
                    }
                    else
                    {
                        return GetTranslatedString(VanillaStrings.CONTEXT_ARCHIVE, SENTENCE_TEMPLATE_DESCRIPTION, description, character, text);
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
            var background = Main.LanguageManager.GetSprite(backgroundRef);
            var musicID = group.archive.music;
            ui.SetSimulationBackground(background);
            ui.DisplayPage(ArchiveUI.Page.Simulation);
            simulationTalk.StartTalk(viewingTalkID, 0, onEnd: ShowReplayDialog);
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
            ui.DisplayPage(ArchiveUI.Page.Details);
        }
        private string GetTranslatedString(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
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
