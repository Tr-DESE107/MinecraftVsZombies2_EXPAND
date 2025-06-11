using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Scenes;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;
using PVZEngine;
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

        #region ˽�з���

        #region ��������
        private void Awake()
        {
            ui.OnReturnClick += OnIndexReturnClickCallback;
            ui.OnMusicItemClick += OnMusicItemClickCallback;
            ui.OnPlayButtonClick += OnPlayButtonClickCallback;
            ui.OnPauseButtonClick += OnPauseButtonClickCallback;
            ui.OnMusicBarDrag += OnMusicBarDragCallback;
            ui.OnMusicBarPointerUp += OnMusicBarPointerUpCallback;
            ui.OnTrackButtonClick += OnTrackButtonClickCallback;
        }
        private void Update()
        {
            UpdateMusic();
        }
        #endregion

        #region �¼��ص�
        private void OnIndexReturnClickCallback()
        {
            Hide();
            Main.MusicManager.Stop();
            Main.MusicManager.SetTrackWeight(0);
            playingSubTrack = false;
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
                playingSubTrack = false;
            }
        }
        private void OnPauseButtonClickCallback()
        {
            Main.MusicManager.Pause();
        }
        private void OnTrackButtonClickCallback()
        {
            if (currentMusicId == Main.MusicManager.GetCurrentMusicID())
            {
                playingSubTrack = !playingSubTrack;
                ui.SetTrackButtonStyle(playingSubTrack);
            }
            else
            {
                ui.SetTrackButtonStyle(false);
            }
        }
        private void OnMusicBarDragCallback(float value)
        {
            var music = Main.MusicManager.GetCurrentMusicID();
            if (currentMusicId != music)
            {
                Main.MusicManager.Play(currentMusicId);
                playingSubTrack = false;
            }
            Main.MusicManager.Pause();
            Main.MusicManager.SetNormalizedMusicTime(value);
        }
        private void OnMusicBarPointerUpCallback()
        {
            if (Main.MusicManager.IsPaused && currentMusicId == Main.MusicManager.GetCurrentMusicID())
            {
                Main.MusicManager.Resume();
            }
            else
            {
                Main.MusicManager.Play(currentMusicId);
                playingSubTrack = false;
            }
            Main.MusicManager.SetNormalizedMusicTime(ui.GetMusicBarValue());
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
                return Main.SaveManager.IsInvalidOrUnlocked(unlockID);
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
                ui.SetMusicTime(0, "00:00");
            }
            else
            {
                var time = Main.MusicManager.Time;
                var timeString = string.Format("{0:00}:{1:00}", (int)time / 60, (int)time % 60);
                ui.SetMusicTime(Main.MusicManager.GetNormalizedMusicTime(), timeString);
            }
            float weightSpeed = playingSubTrack ? 1 : -1;
            subTrackWeight = Mathf.Clamp01(subTrackWeight + weightSpeed * 0.03f);
            Main.MusicManager.SetTrackWeight(subTrackWeight);
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

            var mainTrack = Main.ResourceManager.GetMusicClip(meta.MainTrack);
            string totalTime;
            if (mainTrack != null)
            {
                var time = mainTrack.length;
                totalTime = string.Format("{0:00}:{1:00}", (int)time / 60, (int)time % 60);
            }
            else
            {
                totalTime = "??:??";
            }


            ui.UpdateInformation(name, infoBuilder.ToString(), description, totalTime);
            ui.SetSelectedItem(index);
            ui.SetTrackButtonVisible(NamespaceID.IsValid(meta.SubTrack));
            ui.SetTrackButtonStyle(currentMusicId == Main.MusicManager.GetCurrentMusicID() && playingSubTrack);
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
        [TranslateMsg("�������е���Ϣģ�壬{0}Ϊ������Դ")]
        public const string INFORMATION_SOURCE = "��Դ��{0}";
        [TranslateMsg("�������е���Ϣģ�壬{0}Ϊ����ԭ��")]
        public const string INFORMATION_ORIGIN = "ԭ����{0}";
        [TranslateMsg("�������е���Ϣģ�壬{0}Ϊ��������")]
        public const string INFORMATION_AUTHOR = "���ߣ�{0}";
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> musicList = new List<NamespaceID>();
        private NamespaceID currentMusicId;
        private bool playingSubTrack;
        private float subTrackWeight;

        [SerializeField]
        private MusicRoomUI ui;
    }
}
