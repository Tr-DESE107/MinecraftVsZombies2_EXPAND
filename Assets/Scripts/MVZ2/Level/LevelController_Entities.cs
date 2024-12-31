using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Entities;
using MVZ2.Level.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法
        public EntityController GetEntityController(Entity entity)
        {
            return entities.FirstOrDefault(e => e.Entity == entity);
        }
        private EntityController CreateControllerForEntity(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, LawnToTrans(entity.Position), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            entityController.Init(this, entity);
            entityController.OnPointerEnter += UI_OnEntityPointerEnterCallback;
            entityController.OnPointerExit += UI_OnEntityPointerExitCallback;
            entityController.OnPointerDown += UI_OnEntityPointerDownCallback;
            entities.Add(entityController);
            return entityController;
        }
        private bool RemoveControllerFromEntity(Entity entity)
        {
            var entityController = GetEntityController(entity);
            if (entityController)
            {
                entityController.OnPointerEnter -= UI_OnEntityPointerEnterCallback;
                entityController.OnPointerExit -= UI_OnEntityPointerExitCallback;
                entityController.OnPointerDown -= UI_OnEntityPointerDownCallback;
                Destroy(entityController.gameObject);
                return entities.Remove(entityController);
            }
            return false;
        }

        #endregion

        #region 私有方法

        #region 事件回调
        private void Engine_OnEntitySpawnCallback(Entity entity)
        {
            CreateControllerForEntity(entity);
        }
        private void Engine_OnEntityRemoveCallback(Entity entity)
        {
            RemoveControllerFromEntity(entity);
        }
        private void UI_OnEntityPointerEnterCallback(EntityController entityCtrl, PointerEventData eventData)
        {
            var target = new HeldItemTargetEntity(entityCtrl.Entity);
            var highlight = level.GetHeldHighlight(target);
            entityCtrl.SetHovered(true, highlight == HeldHighlight.Entity);
            if (IsGameRunning())
            {
                // 自动拾取
                if (Input.GetMouseButton((int)MouseButton.LeftMouse))
                {
                    level.UseHeldItem(target, PointerPhase.Enter);
                }
            }
            else
            {
                // 显示查看图鉴提示
                if (entityCtrl.Entity.IsPreviewEnemy() && CanChooseBlueprints())
                {
                    var name = Main.ResourceManager.GetEntityName(entityCtrl.Entity.GetDefinitionID());
                    var description = string.Empty;
                    if (Main.SaveManager.IsAlmanacUnlocked())
                    {
                        description = Main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_TOOLTIP, VIEW_IN_ALMANAC);
                    }
                    var uiPreset = GetUIPreset();
                    uiPreset.ShowTooltipOnComponent(entityCtrl, new TooltipViewData()
                    {
                        name = name,
                        description = description
                    });
                }
            }
        }
        private void UI_OnEntityPointerExitCallback(EntityController entity, PointerEventData eventData)
        {
            entity.SetHovered(false, false);
            // 隐藏查看图鉴提示
            if (entity.Entity.IsPreviewEnemy())
            {
                var uiPreset = GetUIPreset();
                uiPreset.HideTooltip();
            }
        }
        private void UI_OnEntityPointerDownCallback(EntityController entityCtrl, PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            var entity = entityCtrl.Entity;
            if (IsGameRunning())
            {
                var target = new HeldItemTargetEntity(entity);
                level.UseHeldItem(target, PointerPhase.Press);
            }
            else
            {
                // 打开图鉴
                if (entity.IsPreviewEnemy() && Main.SaveManager.IsAlmanacUnlocked() && CanChooseBlueprints())
                {
                    var uiPreset = GetUIPreset();
                    uiPreset.HideTooltip();
                    OpenEnemyAlmanac(entity.GetDefinitionID());
                    Main.SoundManager.Play2D(VanillaSoundID.tap);
                }
            }
        }
        #endregion

        private void UpdateEnemyCry()
        {
            cryTimeCheckTimer.Run();
            if (cryTimeCheckTimer.Expired)
            {
                cryTimeCheckTimer.Reset();

                var crySoundEnemies = GetCrySoundEnemies();
                var enemyCount = crySoundEnemies.Count();
                float t = Mathf.Clamp01((float)(enemyCount - MinEnemyCryCount) / (MaxEnemyCryCount - MinEnemyCryCount));
                maxCryTime = (int)Mathf.Lerp(MaxCryInterval, MinCryInterval, t);
            }
            cryTimer.Run();
            if (cryTimer.MaxFrame - cryTimer.Frame >= maxCryTime)
            {
                cryTimer.Reset();

                var crySoundEnemies = GetCrySoundEnemies();
                var enemyCount = crySoundEnemies.Count();
                if (enemyCount <= 0)
                    return;
                var crySoundEnemy = crySoundEnemies.Random(rng);
                crySoundEnemy.PlaySound(crySoundEnemy.GetCrySound());
            }
        }
        private IEnumerable<Entity> GetCrySoundEnemies()
        {
            var enemies = level.GetEntities(EntityTypes.ENEMY);
            if (enemies.Length <= 0)
                return Enumerable.Empty<Entity>();
            return enemies.Where(e => e.GetCrySound() != null);
        }

        #endregion

        #region 属性字段
        public const int SelfFaction = 0;
        public const int EnemyFaction = 1;
        public const int MinEnemyCryCount = 1;
        public const int MaxEnemyCryCount = 20;
        public const int MinCryInterval = 60;
        public const int MaxCryInterval = 300;

        [TranslateMsg("实体提示", VanillaStrings.CONTEXT_ENTITY_TOOLTIP)]
        public const string VIEW_IN_ALMANAC = "在图鉴中查看";

        private List<EntityController> entities = new List<EntityController>();


        #region 保存属性
        private FrameTimer cryTimer = new FrameTimer(MaxCryInterval);
        private FrameTimer cryTimeCheckTimer = new FrameTimer(7);
        private int maxCryTime = MaxCryInterval;
        #endregion

        [Header("Entities")]
        [SerializeField]
        private EntityController entityTemplate;
        [SerializeField]
        private Transform entitiesRoot;
        #endregion
    }
}
