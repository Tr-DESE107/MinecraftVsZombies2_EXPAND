using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        #region 图鉴
        public bool IsOpeningAlmanac() => isOpeningAlmanac;
        public void OpenAlmanac()
        {
            isOpeningAlmanac = true;
            SetCameraDisabled(true);
            Main.Scene.DisplayAlmanac(() =>
            {
                isOpeningAlmanac = false;
                SetCameraDisabled(false);
                if (!Music.IsPlaying(VanillaMusicID.choosing))
                    Music.Play(VanillaMusicID.choosing);
            });
        }
        private void OpenEnemyAlmanac(NamespaceID enemyID)
        {
            OpenAlmanac();
            Main.Scene.DisplayEnemyAlmanac(enemyID);
        }
        #endregion

        #region 商店
        public bool IsOpeningStore() => isOpeningStore;
        public void OpenStore()
        {
            isOpeningStore = true;
            SetCameraDisabled(true);
            Main.Scene.DisplayStore(() =>
            {
                isOpeningStore = false;
                SetCameraDisabled(false);
                level.UpdatePersistentLevelUnlocks();
                BlueprintChoosePart.Refresh(Saves.GetUnlockedContraptions());
                if (!Music.IsPlaying(VanillaMusicID.choosing))
                    Music.Play(VanillaMusicID.choosing);
            }, false);
        }
        #endregion
        public bool IsOpeningExtraScene() => IsOpeningAlmanac() || IsOpeningStore();

        #region 属性字段
        private bool isOpeningAlmanac;
        private bool isOpeningStore;
        #endregion
    }
}
