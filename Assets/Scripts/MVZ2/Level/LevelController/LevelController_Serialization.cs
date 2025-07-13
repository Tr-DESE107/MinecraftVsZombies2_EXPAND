using System;
using MVZ2.Games;
using MVZ2.Level.Components;
using MVZ2.Level.UI;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        #region 公有方法
        public SerializableLevelControllerHeader SaveGameHeader()
        {
            return new SerializableLevelControllerHeader()
            {
                identifiers = LevelManager.GetLevelStateIdentifierList(),
            };
        }
        public SerializableLevelController SaveGame()
        {
            var seri = new SerializableLevelController()
            {
                rng = rng.ToSerializable(),
                level = level.Serialize(),
            };
            WriteToSerializable_Audio(seri);
            WriteToSerializable_Cry(seri);
            WriteToSerializable_Entities(seri);
            WriteToSerializable_Model(seri);
            WriteToSerializable_Parts(seri);
            WriteToSerializable_ProgressBar(seri);
            WriteToSerializable_Twinkle(seri);
            WriteToSerializable_Tools(seri);
            WriteToSerializable_UI(seri);
            return seri;
        }
        public bool ValidateGameStateHeader(SerializableLevelControllerHeader header)
        {
            var compareResult = LevelManager.GetLevelStateIdentifierList().Compare(header.identifiers);
            if (!compareResult.valid)
            {
                ShowLevelMismatchLoadingDialog(compareResult);
                return false;
            }
            return true;
        }
        public bool LoadGame(SerializableLevelController seri, Game game, NamespaceID areaID, NamespaceID stageID)
        {
            try
            {
                rng = RandomGenerator.FromSerializable(seri.rng);
                level = LevelEngine.Deserialize(seri.level, game, game, game, GetCollisionSystem());
                InitLevelEngine(level, game, areaID, stageID);

                level.DeserializeComponents(seri.level);

                ReadFromSerializable_ProgressBar(seri);
                ReadFromSerializable_Twinkle(seri);
                ReadFromSerializable_Tools(seri);
                ReadFromSerializable_UI(seri);
                ReadFromSerializable_Cry(seri);
                ReadFromSerializable_Audio(seri);
                CreateGridControllers();
                ReadFromSerializable_Parts(seri);
                ReadFromSerializable_Entities(seri);
                ReadFromSerializable_Model(seri);
            }
            catch (Exception e)
            {
                ShowLevelErrorLoadingDialog(e);
                Debug.LogException(e);
                return false;
            }

            // 设置UI可见状态
            SetUIVisibleState(LevelUIPreset.VisibleState.InLevel);
            // 相机位置
            SetCameraPosition(LevelCameraPosition.Lawn);
            UpdateCamera();
            UpdateCameraByLevel(level);

            // 手持物品
            level.ResetHeldItem();
            RefreshUIAtLevelInit();
            RefreshUIAtLevelStart();
            ShowMoney();
            // 光照
            UpdateLighting();

            // 游戏开始状态
            SetGameStarted(true);


            foreach (var component in level.GetComponents())
            {
                if (component is IMVZ2LevelComponent comp)
                {
                    comp.PostLevelLoad();
                }
            }
            foreach (var part in parts)
            {
                part.PostLevelLoad();
            }

            PauseGame();
            ShowLevelLoadedDialog();
            levelLoaded = true;

            level.AreaDefinition.PostLoad(level);

            return true;
        }
        #endregion


        private bool levelLoaded = false;
    }
}
