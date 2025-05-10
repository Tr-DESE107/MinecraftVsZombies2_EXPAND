using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Archives;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.MusicRoom
{
    public class MusicRoomController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            UpdateMusicList();
            Main.MusicManager.Stop();
        }

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            ui.OnReturnClick += OnIndexReturnClickCallback;
            ui.OnMusicItemClick += OnMusicItemClickCallback;
            ui.OnPlayButtonClick += OnPlayButtonClickCallback;
            ui.OnPauseButtonClick += OnPauseButtonClickCallback;
            ui.OnMusicBarDrag += OnMusicBarDragCallback;
        }
        private void Update()
        {
            UpdateMusic();
        }
        #endregion

        #region 事件回调
        private void OnIndexReturnClickCallback()
        {
            Hide();
            Main.MusicManager.Stop();
            OnReturnClick?.Invoke();
        }
        private void OnMusicItemClickCallback(int index)
        {
            DisplayMusic(index);
        }
        private void OnPlayButtonClickCallback()
        {
            if (Main.MusicManager.IsPaused && currentMusicId == Main.MusicManager.GetCurrentMusicID())
            {
                Main.MusicManager.Resume();
            }
            else
            {
                Main.MusicManager.Play(currentMusicId);
            }
        }
        private void OnPauseButtonClickCallback()
        {
            Main.MusicManager.Pause();
        }
        private void OnMusicBarDragCallback(float value)
        {
            var music = Main.MusicManager.GetCurrentMusicID();
            if (currentMusicId != music)
            {
                Main.MusicManager.Play(currentMusicId);
            }
            Main.MusicManager.Pause();
            Main.MusicManager.SetNormalizedMusicTime(value);
        }
        #endregion

        private void UpdateMusicList()
        {
            musicList.Clear();
            var musics = Main.ResourceManager.GetAllMusicID();
            var unlockedMusic = musics.Where(t =>
            {
                var meta = Main.ResourceManager.GetMusicMeta(t);
                var unlockID = meta?.Unlock;
                return !NamespaceID.IsValid(unlockID) || Main.SaveManager.IsUnlocked(unlockID);
            });
            musicList.AddRange(unlockedMusic);

            ui.UpdateList(musicList.Select(m => Main.ResourceManager.GetMusicName(m)).ToArray());

            if (musicList.Count > 0)
            {
                DisplayMusic(0);
            }
        }
        private void UpdateMusic()
        {
            ui.SetPlaying(Main.MusicManager.IsPlaying(currentMusicId) && !Main.MusicManager.IsPaused);
            var music = Main.MusicManager.GetCurrentMusicID();
            if (music != currentMusicId)
            {
                ui.SetMusicTime(0);
            }
            else
            {
                ui.SetMusicTime(Main.MusicManager.GetNormalizedMusicTime());
            }
        }
        private void DisplayMusic(int index)
        {
            currentMusicId = musicList[index];
            var meta = Main.ResourceManager.GetMusicMeta(currentMusicId);
            var name = Main.ResourceManager.GetMusicName(currentMusicId);
            var infoBuilder = new StringBuilder();
            var sourceKey = meta.Source;
            if (!string.IsNullOrEmpty(sourceKey))
            {
                var source = GetTranslatedStringParticular(VanillaStrings.CONTEXT_MUSIC_SOURCE, sourceKey);
                infoBuilder.AppendLine(GetTranslatedString(INFORMATION_SOURCE, source));
            }
            var originKey = meta.Origin;
            if (!string.IsNullOrEmpty(originKey))
            {
                var origin = GetTranslatedStringParticular(VanillaStrings.CONTEXT_MUSIC_ORIGIN, originKey);
                infoBuilder.AppendLine(GetTranslatedString(INFORMATION_ORIGIN, origin));
            }
            var authorKey = meta.Author;
            if (!string.IsNullOrEmpty(authorKey))
            {
                var author = GetTranslatedStringParticular(VanillaStrings.CONTEXT_MUSIC_AUTHOR, authorKey);
                infoBuilder.AppendLine(GetTranslatedString(INFORMATION_AUTHOR, author));
            }

            var description = GetTranslatedStringParticular(VanillaStrings.CONTEXT_MUSIC_DESCRIPTION, meta.Description);

            ui.UpdateInformation(name, infoBuilder.ToString(), description);
            ui.SetSelectedItem(index);
        }
        private string GetTranslatedString(string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._(text, args);
        }
        private string GetTranslatedStringParticular(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
        #endregion
        public event Action OnReturnClick;
        [TranslateMsg("音乐室中的信息模板，{0}为音乐来源")]
        public const string INFORMATION_SOURCE = "来源：{0}";
        [TranslateMsg("音乐室中的信息模板，{0}为音乐原曲")]
        public const string INFORMATION_ORIGIN = "原曲：{0}";
        [TranslateMsg("音乐室中的信息模板，{0}为音乐作者")]
        public const string INFORMATION_AUTHOR = "作者：{0}";
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> musicList = new List<NamespaceID>();
        private NamespaceID currentMusicId;

        [SerializeField]
        private MusicRoomUI ui;
    }
}
