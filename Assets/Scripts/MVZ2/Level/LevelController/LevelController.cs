using System;
using System.Linq;
using MVZ2.Audios;
using MVZ2.Cameras;
using MVZ2.Games;
using MVZ2.Level.Components;
using MVZ2.Localization;
using MVZ2.Managers;
using MVZ2.Options;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2Logic.Level;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, ILevelController, IDisposable
    {
        private void Awake()
        {
            parts = new ILevelControllerPart[]
            {
                blueprintController,
                blueprintChooseController,
            };
            Awake_Collision();
            Awake_Model();
            Awake_Grids();
            Awake_Entities();
            Awake_Talk();
            Awake_Camera();
            Awake_HeldItem();
            Awake_ProgressBar();
            Awake_Tooltip();
            Awake_Dialogs();
            Awake_Tools();
            Awake_Artifacts();
            Awake_Blueprints();
            Awake_UI();
            foreach (var controller in parts)
            {
                controller.Init(this);
            }
        }
        private void WriteToSerializable_Parts(SerializableLevelController seri)
        {
            seri.parts = parts.Select(p => p.ToSerializable()).ToArray();
        }
        private void ReadFromSerializable_Parts(SerializableLevelController seri)
        {
            foreach (var part in parts)
            {
                var seriPart = seri.parts.FirstOrDefault(p => p.id == part.ID);
                if (seriPart == null)
                {
                    Debug.LogWarning($"Could not find serialized LevelControllerPart data with id {part.ID}.");
                    continue;
                }
                part.LoadFromSerializable(seriPart);
            }
        }
        public void Dispose()
        {
            if (optionsLogic != null)
            {
                optionsLogic.Dispose();
                optionsLogic = null;
            }
            Music.SetVolume(1);
            Music.SetTrackWeight(0);
            if (level != null)
            {
                foreach (var component in level.GetComponents())
                {
                    if (component is IMVZ2LevelComponent comp)
                    {
                        comp.PostDispose();
                    }
                }
                level.StopAllLoopSounds();
                level.Dispose();
            }
            Game.SetLevel(null);
            isDisposed = true;
        }
        public void UpdateDifficulty()
        {
            if (level.CurrentFlag <= 0)
            {
                level.SetDifficulty(Options.GetDifficulty());
            }
            UpdateDifficultyName();
        }
        public int GetCurrentFlag()
        {
            return level.CurrentFlag;
        }
        public LevelEngine GetEngine()
        {
            return level;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        #region 属性字段
        public Game Game => Main.Game;
        private MainManager Main => MainManager.Instance;
        private SaveManager Saves => Main.SaveManager;
        private MusicManager Music => Main.MusicManager;
        private LevelManager LevelManager => Main.LevelManager;
        private LanguageManager Localization => Main.LanguageManager;
        private ResourceManager Resources => Main.ResourceManager;
        private SoundManager Sounds => Main.SoundManager;
        private MainSceneController Scene => Main.Scene;
        private OptionsManager Options => Main.OptionsManager;
        private ShakeManager Shakes => Main.ShakeManager;
        private LevelEngine level;
        private RandomGenerator rng;
        private bool isDisposed;

        private ILevelControllerPart[] parts;
        #endregion
    }
}
