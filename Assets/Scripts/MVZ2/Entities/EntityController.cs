using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Cursors;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Entities
{
    public class EntityController : MonoBehaviour, ILevelRaycastReceiver, ITooltipTarget
    {
        #region 公有方法
        public void Init(LevelController level, Entity entity)
        {
            Level = level;
            Entity = entity;
            rng = new RandomGenerator(entity.InitSeed);
            isHighlight = false;
            gameObject.name = entity.ToString();
            transform.position = Level.LawnToTrans(Entity.Position);
            lastPosition = transform.position;

            entity.PostInit += PostInitCallback;
            entity.PostPropertyChanged += PostPropertyChangedCallback;
            entity.OnChangeModel += OnChangeModelCallback;

            entity.OnEquipArmor += OnArmorEquipCallback;
            entity.OnRemoveArmor += OnArmorRemoveCallback;

            holdStreakHandler.ResetData();
            RemoveCursorSource();

            entity.SetModelInterface(bodyModelInterface);
            SetModel(Entity.ModelID);
            if (Model)
            {
                ClearAllArmorModels();
                var lvl = Entity.Level;
                var heldItemDef = lvl.GetHeldItemDefinition();
                UpdateModelColliderActive(heldItemDef?.GetHeldTargetMask(lvl) ?? HeldTargetFlag.None);
            }
        }
        public void RemoveEntity()
        {
            Entity.PostInit -= PostInitCallback;
            Entity.PostPropertyChanged -= PostPropertyChangedCallback;
            Entity.OnChangeModel -= OnChangeModelCallback;

            Entity.OnEquipArmor -= OnArmorEquipCallback;
            Entity.OnRemoveArmor -= OnArmorRemoveCallback;
        }

        #region 模型
        public void SetModel(NamespaceID modelId)
        {
            if (Model)
            {
                Destroy(Model.gameObject);
                Model.OnUpdateFrame -= OnModelUpdateFrameCallback;
                Model = null;
            }
            var model = Models.Model.Create(modelId, transform, Level.GetCamera(), Entity.InitSeed);
            Model = model as SpriteModel;
            if (!Model)
                return;
            Model.OnUpdateFrame += OnModelUpdateFrameCallback;
            modelPropertyCache.UpdateAll(this);
            Model.UpdateFrame(0);
            Model.UpdateAnimators(0);
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            if (Model)
            {
                Model.SetSimulationSpeed(simulationSpeed);
            }
        }
        #endregion

        #region 更新
        public void UpdateFixed()
        {
            if (Model)
            {
                Model.UpdateFixed();
            }
            holdStreakHandler.UpdateHoldAndStreak();
        }
        public void UpdateFrame(float deltaTime)
        {
            var posOffset = GetTransformOffset();

            var pos = Entity.Position;
            var currentTransPos = Level.LawnToTrans(pos);
            transform.position = Vector3.Lerp(lastPosition, currentTransPos + posOffset, 0.5f);
            var finalOffset = transform.position - currentTransPos;
            lastPosition = transform.position;

            UpdateShadow(finalOffset);
            var shouldTwinkle = ShouldTwinkle();
            if (twinkling != shouldTwinkle)
            {
                twinkling = shouldTwinkle;
                modelPropertyCache.SetDirtyProperty(EntityPropertyCache.PropertyName.Tint);
            }
            else if (twinkling)
            {
                modelPropertyCache.SetDirtyProperty(EntityPropertyCache.PropertyName.Tint);
            }
            if (Model)
            {
                Model.UpdateFrame(deltaTime);
            }
        }
        public void UpdateAnimators(float deltaTime)
        {
            if (Model)
            {
                Model.UpdateAnimators(deltaTime);
            }
        }
        public void GetAnimatorsToUpdate(IList<Animator> results)
        {
            if (Model)
            {
                Model.GetAnimatorsToUpdate(results);
            }
        }
        #endregion

        public void UpdateModelColliderActive(HeldTargetFlag flag)
        {
            if (Model is SpriteModel sprModel)
            {
                sprModel.SetColliderActive((flag & sprModel.HeldTargetFlag) != HeldTargetFlag.None);
            }
        }
        public void SetHighlight(bool highlight)
        {
            isHighlight = highlight;
            modelPropertyCache.SetDirtyProperty(EntityPropertyCache.PropertyName.ColorOffset);
        }
        public bool IsHovered() => holdStreakHandler.IsHovered();
        public bool IsPressed() => holdStreakHandler.IsPressed();
        public int GetHoveredPointerCount() => holdStreakHandler.GetHoveredPointerCount();
        public PointerEventData GetHoveredPointerEventData(int index) => holdStreakHandler.GetHoveredPointerEventData(index);
        public Vector2 TransformWorld2ColliderPosition(Vector3 worldPosition)
        {
            if (Model is not SpriteModel spriteModel)
                return Vector2.zero;
            var collider = spriteModel.Collider;
            if (collider is not BoxCollider2D boxCollider)
                return Vector2.zero;
            var pos2D = (Vector2)(collider.transform.position + collider.transform.TransformDirection(collider.offset));
            var lossyScale = (Vector2)collider.transform.lossyScale;
            var size = boxCollider.size;
            var lossySize = Vector2.Scale(size, lossyScale);
            var origin = pos2D - lossySize * 0.5f;

            var relativeWorldPos = (Vector2)worldPosition - origin;
            var colliderX = relativeWorldPos.x / lossySize.x;
            var colliderY = relativeWorldPos.y / lossySize.y;

            return new Vector2(colliderX, colliderY);
        }
        public HeldItemTargetEntity GetHeldItemTarget(Vector3 worldPosition)
        {
            var pos = TransformWorld2ColliderPosition(worldPosition);
            return new HeldItemTargetEntity(Entity, pos);
        }
        public HeldItemTargetEntity GetHeldItemTarget(PointerEventData data)
        {
            var pos = Vector2.zero;
            if (data != null)
            {
                pos = TransformWorld2ColliderPosition(data.pointerCurrentRaycast.worldPosition);
            }
            return new HeldItemTargetEntity(Entity, pos);
        }
        public SerializableEntityController ToSerializable()
        {
            return new SerializableEntityController()
            {
                id = Entity.ID,
                model = Model ? Model.ToSerializable() : null
            };
        }
        public void LoadFromSerializable(SerializableEntityController serializable)
        {
            if (Model && serializable.model != null)
            {
                Model.LoadFromSerializable(serializable.model);
            }
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void Awake()
        {
            holdStreakHandler.OnPointerInteraction += (_, d, i) => OnPointerInteraction?.Invoke(this, d, i);
            bodyModelInterface = new BodyModelInterface(this);
        }
        private void Update()
        {
            var engine = Entity.Level;
            bool cursorValid = IsHovered() && Level.IsGameRunning() && !engine.IsHoldingItem();
            if (cursorValid)
            {
                AddCursorSource();
            }
            else
            {
                RemoveCursorSource();
            }
        }
        #endregion

        #region 事件回调
        private void PostInitCallback()
        {
            UpdateFrame(0);
            UpdateAnimators(0);
        }
        private void PostPropertyChangedCallback(IPropertyKey key, object beforeValue, object afterValue)
        {
            modelPropertyCache.SetDirtyProperty(key);
        }
        private void OnChangeModelCallback(NamespaceID modelID)
        {
            SetModel(modelID);
        }
        private void OnArmorEquipCallback(NamespaceID slot, Armor armor)
        {
            CreateArmorModel(slot, armor);
        }
        private void OnArmorRemoveCallback(NamespaceID slot, Armor armor)
        {
            RemoveArmorModel(slot);
        }
        private void OnModelUpdateFrameCallback(float deltaTime)
        {
            UpdateArmorModels();
            UpdateEntityModel();
        }
        #endregion

        #region 接口实现
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data, PointerEventData d)
        {
            if (Entity.IsPreviewEnemy())
            {
                return true;
            }
            if (definition == null)
                return false;
            var target = GetHeldItemTarget(d);
            var pointer = InputManager.GetPointerDataFromEventData(d);
            return definition.IsValidFor(target, data, pointer);
        }
        int ILevelRaycastReceiver.GetSortingLayer()
        {
            return Model.RendererGroup.SortingLayerID;
        }
        int ILevelRaycastReceiver.GetSortingOrder()
        {
            return Model.RendererGroup.SortingOrder;
        }
        #endregion

        #region 位置
        protected void UpdateShadow(Vector3 posOffset)
        {
            var pos = Entity.Position;
            var groundY = Entity.GetGroundY();
            var relativeY = pos.y - groundY;
            var shadowPos = pos;
            shadowPos.y = groundY;
            shadowPos += modelPropertyCache.ShadowOffset;

            float scale = Mathf.Max(0, 1 + relativeY / 300);
            float alpha = Mathf.Clamp01(1 - relativeY / 300);
            var shadowTransform = Shadow.transform;
            shadowTransform.position = Level.LawnToTrans(shadowPos) + posOffset;
            shadowTransform.localScale = modelPropertyCache.ShadowScale * scale;
            Shadow.gameObject.SetActive(!modelPropertyCache.ShadowHidden);
            Shadow.SetAlpha(modelPropertyCache.ShadowAlpha * alpha);
        }
        protected float GetZOffset()
        {
            float zOffset = 0;
            if (zOffsetDict.TryGetValue(Entity.Type, out float offset))
            {
                zOffset = offset * Level.LawnToTransScale;
            }
            return zOffset;
        }
        protected Vector3 GetTransformOffset()
        {
            float zOffset = GetZOffset();
            return Vector3.back * zOffset;
        }
        #endregion

        #region 护甲
        private void CreateArmorModel(NamespaceID slot, Armor armor)
        {
            if (!Model)
                return;
            if (armor?.Definition == null)
                return;
            var modelID = armor.Definition.GetModelID();
            if (!NamespaceID.IsValid(modelID))
                return;
            var anchor = GetArmorModelAnchor(armor.Definition.GetID(), slot);
            if (string.IsNullOrEmpty(anchor))
                return;
            Model.CreateArmor(anchor, slot, modelID);
        }
        public string GetArmorModelAnchor(NamespaceID armorID, NamespaceID slotID)
        {
            var slotMeta = Main.ResourceManager.GetArmorSlotMeta(slotID);
            if (slotMeta == null)
                return null;

            var armorMeta = Main.ResourceManager.GetArmorMeta(armorID);
            if (armorMeta != null)
            {
                var tagAnchorMeta = slotMeta.Anchors.FirstOrDefault(a => armorMeta.Tags.Contains(a.Tag));
                if (tagAnchorMeta != null)
                {
                    return tagAnchorMeta.Anchor;
                }
            }

            return slotMeta.Anchors.FirstOrDefault()?.Anchor;
        }
        private void RemoveArmorModel(NamespaceID slot)
        {
            if (!Model)
                return;
            Model.RemoveArmor(slot);
        }
        private void UpdateArmorModel(NamespaceID slot)
        {
            if (!Model)
                return;
            var armor = Entity.GetArmorAtSlot(slot);
            if (armor == null)
                return;
            var armorModel = Model.GetArmorModel(slot);
            if (!armorModel)
                return;
            armorModel.GraphicGroup.SetTint(armor.GetTint());
            armorModel.GraphicGroup.SetColorOffset(armor.GetColorOffset());
        }
        private void UpdateArmorModels()
        {
            if (!Model)
                return;
            foreach (var slotSlot in Entity.GetActiveArmorSlots())
            {
                UpdateArmorModel(slotSlot);
            }
        }
        public void ClearAllArmorModels()
        {
            var slotsID = Main.ResourceManager.GetAllArmorSlots();
            foreach (var slotID in slotsID)
            {
                var meta = Main.ResourceManager.GetArmorSlotMeta(slotID);
                if (meta == null)
                    continue;
                var anchorMetas = meta.Anchors;
                foreach (var anchorMeta in anchorMetas)
                {
                    Model.ClearModelAnchor(anchorMeta.Anchor);
                }
            }
        }
        #endregion

        #region 模型
        private void UpdateEntityModel()
        {
            if (!Model)
                return;

            if (Level.IsGameOver() && (Entity.Type == EntityTypes.ENEMY || Entity.Type == EntityTypes.BOSS))
            {
                Model.SetAnimatorInt("State", VanillaEntityStates.WALK);
            }
            var groundPos = Entity.Position;
            groundPos.y = Entity.GetGroundY();
            var transGroundPos = Level.LawnToTrans(groundPos);
            Model.SetGroundY(transGroundPos.y);
            Model.GetCenterTransform().localEulerAngles = Entity.RenderRotation;

            if (modelPropertyCache.IsDirty)
            {
                modelPropertyCache.Update(this);
            }
        }
        #endregion

        private void AddCursorSource()
        {
            if (_cursorSource == null)
            {
                _cursorSource = new EntityCursorSource(this, CursorType.Point);
                Main.CursorManager.AddCursorSource(_cursorSource);
            }
        }
        private void RemoveCursorSource()
        {
            if (_cursorSource != null)
            {
                Main.CursorManager.RemoveCursorSource(_cursorSource);
                _cursorSource = null;
            }
        }

        private bool ShouldTwinkle()
        {
            var engine = Entity.Level;
            return engine.ShouldHeldItemMakeEntityTwinkle(Entity);
        }
        private Color GetTint()
        {
            var tint = Entity.GetTint();
            if (twinkling)
            {
                tint *= Level.GetTwinkleColor();
            }
            return tint;
        }
        private Color GetColorOffset()
        {
            var color = Entity.GetColorOffset();
            if (isHighlight)
            {
                color = ColorCalculator.Blend(new Color(1, 1, 1, 0.5f), color, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha);
            }
            return color;
        }
        #endregion

        #region View
        public void TriggerView(string name)
        {
            if (Model)
                Model.TriggerAnimator(name);
        }
        public void SetViewBool(string name, bool value)
        {
            if (Model)
                Model.SetAnimatorBool(name, value);
        }
        public void SetViewInt(string name, int value)
        {
            if (Model)
                Model.SetAnimatorInt(name, value);
        }
        public void SetViewFloat(string name, float value)
        {
            if (Model)
                Model.SetAnimatorFloat(name, value);
        }
        #endregion

        #region 事件
        public event Action<EntityController, PointerEventData, PointerInteraction> OnPointerInteraction;
        #endregion

        #region 属性字段
        public static readonly Dictionary<int, float> zOffsetDict = new Dictionary<int, float>()
        {
            { EntityTypes.PLANT, 0 },
            { EntityTypes.OBSTACLE, 0 },
            { EntityTypes.BOSS, 2 },
            { EntityTypes.ENEMY, 3 },
            { EntityTypes.PROJECTILE, 4 },
            { EntityTypes.CART, 5 },
            { EntityTypes.EFFECT, 6 },
            { EntityTypes.PICKUP, 7 },
        };
        public MainManager Main => MainManager.Instance;
        public SpriteModel Model { get; private set; }
        public ShadowController Shadow => shadow;
        public Entity Entity { get; private set; }
        public LevelController Level { get; private set; }
        private RandomGenerator rng;
        private bool isHighlight;
        private bool twinkling;
        private EntityCursorSource _cursorSource;
        private Vector3 lastPosition;
        private IModelInterface bodyModelInterface;
        private EntityPropertyCache modelPropertyCache = new EntityPropertyCache();
        [SerializeField]
        private ShadowController shadow;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
        [SerializeField]
        private LevelPointerInteractionHandler holdStreakHandler;

        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;

        #endregion

        #region 内嵌类

        private class EntityPropertyCache
        {
            public void UpdateAll(EntityController entityCtrl)
            {
                var entity = entityCtrl.Entity;
                var model = entityCtrl.Model;
                var rendererGroup = model.RendererGroup;
                rendererGroup.SetTint(entityCtrl.GetTint());
                rendererGroup.SetHSV(entity.GetHSV());
                rendererGroup.SetColorOffset(entityCtrl.GetColorOffset());
                rendererGroup.SetShaderInt("_Grayscale", entity.IsGrayscale() ? 1 : 0);

                model.transform.localScale = entity.GetFinalDisplayScale();
                rendererGroup.SortingLayerID = SortingLayer.NameToID(entity.GetSortingLayer());
                rendererGroup.SortingOrder = entity.GetSortingOrder();
                if (model is SpriteModel sprModel)
                {
                    sprModel.SetLightVisible(entity.IsLightSource());
                    sprModel.SetLightColor(entity.GetLightColor());
                    var lightScaleLawn = entity.GetLightRange();
                    var lightScale = new Vector2(lightScaleLawn.x, Mathf.Max(lightScaleLawn.y, lightScaleLawn.z)) * entityCtrl.Level.LawnToTransScale;
                    sprModel.SetLightRange(lightScale);
                }

                ShadowHidden = entity.IsShadowHidden();
                ShadowAlpha = entity.GetShadowAlpha();
                ShadowOffset = entity.GetShadowOffset();
                ShadowScale = entity.GetShadowScale();

                dirtyProperties.Clear();
            }
            public void Update(EntityController entityCtrl)
            {
                var entity = entityCtrl.Entity;
                var model = entityCtrl.Model;
                var rendererGroup = model.RendererGroup;
                foreach (var dirtyProperty in dirtyProperties)
                {
                    switch (dirtyProperty)
                    {
                        case PropertyName.Tint:
                            rendererGroup.SetTint(entityCtrl.GetTint());
                            break;
                        case PropertyName.ColorOffset:
                            rendererGroup.SetColorOffset(entityCtrl.GetColorOffset());
                            break;
                        case PropertyName.HSV:
                            rendererGroup.SetHSV(entity.GetHSV());
                            break;
                        case PropertyName.Grayscale:
                            rendererGroup.SetShaderInt("_Grayscale", entity.IsGrayscale() ? 1 : 0);
                            break;
                        case PropertyName.FlipX:
                        case PropertyName.DisplayScale:
                            model.transform.localScale = entity.GetFinalDisplayScale();
                            break;
                        case PropertyName.SortingLayer:
                            rendererGroup.SortingLayerID = SortingLayer.NameToID(entity.GetSortingLayer());
                            break;
                        case PropertyName.SortingOrder:
                            rendererGroup.SortingOrder = entity.GetSortingOrder();
                            break;

                        case PropertyName.ShadowHidden:
                            ShadowHidden = entity.IsShadowHidden();
                            break;
                        case PropertyName.ShadowAlpha:
                            ShadowAlpha = entity.GetShadowAlpha();
                            break;
                        case PropertyName.ShadowOffset:
                            ShadowOffset = entity.GetShadowOffset();
                            break;
                        case PropertyName.ShadowScale:
                            ShadowScale = entity.GetShadowScale();
                            break;

                        case PropertyName.LightSource:
                            {
                                if (model is SpriteModel sprModel)
                                {
                                    sprModel.SetLightVisible(entity.IsLightSource());
                                }
                            }
                            break;
                        case PropertyName.LightColor:
                            {
                                if (model is SpriteModel sprModel)
                                {
                                    sprModel.SetLightColor(entity.GetLightColor());
                                }
                            }
                            break;
                        case PropertyName.LightRange:
                            {
                                if (model is SpriteModel sprModel)
                                {
                                    var lightScaleLawn = entity.GetLightRange();
                                    var lightScale = new Vector2(lightScaleLawn.x, Mathf.Max(lightScaleLawn.y, lightScaleLawn.z)) * entityCtrl.Level.LawnToTransScale;
                                    sprModel.SetLightRange(lightScale);
                                }
                            }
                            break;
                    }
                }
                dirtyProperties.Clear();
            }
            public void SetDirtyProperty(PropertyName property)
            {
                dirtyProperties.Add(property);
            }
            public void SetDirtyProperty(IPropertyKey key)
            {
                foreach (var pair in propertyMap)
                {
                    if (pair.Key.Equals(key))
                    {
                        dirtyProperties.Add(pair.Value);
                        break;
                    }
                }
            }
            public bool IsDirty => dirtyProperties.Count > 0;
            public bool ShadowHidden { get; private set; }
            public Vector3 ShadowOffset { get; private set; }
            public Vector3 ShadowScale { get; private set; }
            public float ShadowAlpha { get; private set; }
            private HashSet<PropertyName> dirtyProperties = new HashSet<PropertyName>();
            private static readonly Dictionary<PropertyMeta, PropertyName> propertyMap = new Dictionary<PropertyMeta, PropertyName>()
            {
                { EngineEntityProps.TINT, PropertyName.Tint },
                { EngineEntityProps.COLOR_OFFSET, PropertyName.ColorOffset },
                { VanillaEntityProps.HSV, PropertyName.HSV },
                { VanillaEntityProps.GRAYSCALE, PropertyName.Grayscale },
                { EngineEntityProps.FLIP_X, PropertyName.FlipX },
                { EngineEntityProps.DISPLAY_SCALE, PropertyName.DisplayScale },
                { VanillaEntityProps.SORTING_LAYER, PropertyName.SortingLayer },
                { VanillaEntityProps.SORTING_ORDER, PropertyName.SortingOrder },

                { VanillaEntityProps.SHADOW_HIDDEN, PropertyName.ShadowHidden },
                { VanillaEntityProps.SHADOW_OFFSET, PropertyName.ShadowOffset },
                { VanillaEntityProps.SHADOW_SCALE, PropertyName.ShadowScale },
                { VanillaEntityProps.SHADOW_ALPHA, PropertyName.ShadowAlpha },

                { VanillaEntityProps.IS_LIGHT_SOURCE, PropertyName.LightSource },
                { VanillaEntityProps.LIGHT_COLOR, PropertyName.LightColor },
                { VanillaEntityProps.LIGHT_RANGE, PropertyName.LightRange },
            };
            public enum PropertyName
            {
                Tint,
                ColorOffset,
                HSV,
                Grayscale,
                FlipX,
                DisplayScale,
                SortingLayer,
                SortingOrder,

                ShadowHidden,
                ShadowOffset,
                ShadowScale,
                ShadowAlpha,

                LightSource,
                LightColor,
                LightRange,
            }
        }
        #endregion
    }
    public class EntityCursorSource : CursorSource
    {
        public EntityCursorSource(EntityController target, CursorType type, int priority = 0)
        {
            this.target = target;
            this.type = type;
            this.priority = priority;
        }

        public override bool IsValid()
        {
            return target && target.isActiveAndEnabled;
        }

        public EntityController target;
        private int priority;
        public override int Priority => priority;
        private CursorType type;
        public override CursorType CursorType => type;
    }
    public class SerializableEntityController
    {
        public long id;
        public SerializableModelData model;
    }
}
