using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Collisions;
using MVZ2.Entities;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Supporters;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level.Collisions;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public partial class LevelController : MonoBehaviour, IDisposable
    {
        #region 公有方法
        public EntityController GetEntityController(Entity entity)
        {
            return entities.FirstOrDefault(e => e.Entity == entity);
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
        private void Engine_PostUseEntityBlueprintCallback(VanillaLevelCallbacks.PostUseEntityBlueprintParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            var seed = param.blueprint;
            var definition = param.definition;
            var heldData = param.heldData;
            if (!Main.OptionsManager.ShowSponsorNames())
                return;
            if (entity.IsEntityOf(VanillaContraptionID.furnace))
            {
                ShowFurnaceSponsorName(entity);
            }
            else if (entity.IsEntityOf(VanillaContraptionID.moonlightSensor))
            {
                ShowMoonlightSensorSponsorName(entity);
            }
        }
        private void UI_OnEntityPointerInteractionCallback(EntityController entityCtrl, PointerEventData eventData, PointerInteraction interaction)
        {
            if (IsGameRunning())
            {
                // 触发手持物品指针事件。
                var target = entityCtrl.GetHeldItemTarget();
                var pointerParams = InputManager.GetPointerInteractionParamsFromEventData(eventData, interaction);
                level.DoHeldItemPointerEvent(target, pointerParams);
            }

            if (interaction == PointerInteraction.Enter) // 指针进入
            {
                SetHoveredEntity(entityCtrl);
                if (!IsGameRunning())
                {
                    // 显示查看图鉴提示
                    if (entityCtrl.Entity.IsPreviewEnemy() && CanChooseBlueprints())
                    {
                        ShowTooltip(new EntityTooltipSource(this, entityCtrl));
                    }
                }
            }
            else if (interaction == PointerInteraction.Exit) // 指针退出
            {
                SetHoveredEntity(null);
                // 隐藏查看图鉴提示
                if (entityCtrl.Entity.IsPreviewEnemy())
                {
                    HideTooltip();
                }
            }
            else if (interaction == PointerInteraction.Down) // 指针按下
            {
                if (!IsGameRunning())
                {
                    var pointer = InputManager.GetPointerDataFromEventData(eventData);
                    var entity = entityCtrl.Entity;
                    if (pointer.type != PointerTypes.MOUSE || pointer.button == MouseButtons.LEFT)
                    {
                        // 打开图鉴
                        if (entity.IsPreviewEnemy() && Main.SaveManager.IsAlmanacUnlocked() && CanChooseBlueprints())
                        {
                            var entityID = entityCtrl.Entity.GetDefinitionID();
                            if (Main.ResourceManager.IsEnemyInAlmanac(entityID) && Main.SaveManager.IsEnemyUnlocked(entityID))
                            {
                                HideTooltip();
                                OpenEnemyAlmanac(entity.GetDefinitionID());
                                Main.SoundManager.Play2D(VanillaSoundID.tap);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 叫声
        private void UpdateEnemyCry()
        {
            if (level.IsTimeInterval(7))
            {
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
                crySoundEnemy.PlayCrySound(crySoundEnemy.GetCrySound());
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

        #region 控制器
        private EntityController CreateControllerForEntity(Entity entity)
        {
            var entityController = Instantiate(entityTemplate.gameObject, LawnToTrans(entity.Position), Quaternion.identity, entitiesRoot).GetComponent<EntityController>();
            entityController.Init(this, entity);
            entityController.OnPointerInteraction += UI_OnEntityPointerInteractionCallback;
            entities.Add(entityController);
            return entityController;
        }
        private bool RemoveControllerFromEntity(Entity entity)
        {
            var entityController = GetEntityController(entity);
            if (entityController)
            {
                entityController.OnPointerInteraction -= UI_OnEntityPointerInteractionCallback;
                Destroy(entityController.gameObject);
                return entities.Remove(entityController);
            }
            return false;
        }
        #endregion

        #region 赞助者
        private void ShowFurnaceSponsorName(Entity furnace)
        {
            var names = Main.SponsorManager.GetSponsorPlanNames(SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.FURNACE);
            if (names.Length <= 0)
                return;
            var text = furnace.Spawn(VanillaEffectID.floatingText, furnace.GetCenter(), rng.Next());
            var name = names.Random(text.RNG);
            FloatingText.SetText(text, name);
        }
        private void ShowMoonlightSensorSponsorName(Entity sensor)
        {
            var names = Main.SponsorManager.GetSponsorPlanNames(SponsorPlans.Sensor.TYPE, SponsorPlans.Sensor.MOONLIGHT_SENSOR);
            if (names.Length <= 0)
                return;
            var text = sensor.Spawn(VanillaEffectID.floatingText, sensor.GetCenter(), rng.Next());
            var name = names.Random(text.RNG);
            FloatingText.SetText(text, name);
        }
        #endregion

        private void SetHoveredEntity(EntityController entity)
        {
            hoveredEntity = entity;
            UpdateEntityHighlight();
        }
        private void UpdateEntityHighlight()
        {
            if (!hoveredEntity || hoveredEntity.GetHoveredPointerCount() <= 0)
            {
                SetHighlightedEntity(null);
                return;
            }
            var eventData = hoveredEntity.GetHoveredPointerEventData(0);
            var pointerId = eventData.pointerId;
            var pointerPosition = Main.InputManager.GetPointerPosition(pointerId);
            var worldPosition = levelCamera.Camera.ScreenToWorldPoint(pointerPosition);
            var target = hoveredEntity.GetHeldItemTarget(worldPosition);
            var pointerParams = InputManager.GetPointerDataFromEventData(eventData);
            var highlight = level.GetHeldHighlight(target, pointerParams);
            if (highlight.mode == HeldHighlightMode.Entity)
            {
                var targetEntity = highlight.entity;
                if (targetEntity != null)
                {
                    var ctrl = GetEntityController(targetEntity);
                    SetHighlightedEntity(ctrl);
                }
            }
        }
        private void SetHighlightedEntity(EntityController entity)
        {
            if (highlightedEntity)
            {
                highlightedEntity.SetHighlight(false);
            }
            highlightedEntity = entity;
            if (highlightedEntity)
            {
                highlightedEntity.SetHighlight(true);
            }
        }

        private ICollisionSystem GetCollisionSystem()
        {
            return unityCollisionSystem;
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
        private EntityController hoveredEntity;
        private EntityController highlightedEntity;

        #region 保存属性
        private FrameTimer cryTimer = new FrameTimer(MaxCryInterval);
        private int maxCryTime = MaxCryInterval;
        #endregion

        [Header("Entities")]
        [SerializeField]
        private EntityController entityTemplate;
        [SerializeField]
        private Transform entitiesRoot;
        [SerializeField]
        private UnityCollisionSystem unityCollisionSystem;
        #endregion

        private class EntityTooltipSource : ITooltipSource
        {
            private LevelController controller;
            private EntityController entityCtrl;

            public EntityTooltipSource(LevelController controller, EntityController entityCtrl)
            {
                this.controller = controller;
                this.entityCtrl = entityCtrl;
            }

            public ITooltipTarget GetTarget(LevelController level)
            {
                return entityCtrl;
            }

            public TooltipViewData GetViewData(LevelController level)
            {
                var main = controller.Main;
                var name = main.ResourceManager.GetEntityName(entityCtrl.Entity.GetDefinitionID());
                var description = string.Empty;
                if (main.SaveManager.IsAlmanacUnlocked())
                {
                    var entityID = entityCtrl.Entity.GetDefinitionID();
                    if (main.ResourceManager.IsEnemyInAlmanac(entityID) && main.SaveManager.IsEnemyUnlocked(entityID))
                    {
                        description = main.LanguageManager._p(VanillaStrings.CONTEXT_ENTITY_TOOLTIP, VIEW_IN_ALMANAC);
                    }
                }
                return new TooltipViewData()
                {
                    name = name,
                    description = description
                };
            }
        }
    }
}
