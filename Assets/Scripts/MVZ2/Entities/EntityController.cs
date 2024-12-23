using System;
using System.Collections.Generic;
using MVZ2.Cursors;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2.Level.UI;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Entities
{
    public class EntityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ILevelRaycastReceiver, ITooltipTarget
    {
        #region 公有方法
        public void Init(LevelController level, Entity entity)
        {
            Level = level;
            Entity = entity;
            rng = new RandomGenerator(entity.InitSeed);
            gameObject.name = entity.GetDefinitionID().ToString();
            entity.PostInit += PostInitCallback;
            entity.OnChangeModel += OnChangeModelCallback;

            entity.OnEquipArmor += OnArmorEquipCallback;
            entity.OnDestroyArmor += OnArmorDestroyCallback;
            entity.OnRemoveArmor += OnArmorRemoveCallback;

            bodyModelInterface = new BodyModelInterface(this);
            armorModelInterface = new ArmorModelInterface(this);
            entity.SetModelInterface(bodyModelInterface, armorModelInterface);
            SetModel(Entity.ModelID);

            transform.position = Level.LawnToTrans(Entity.Position);
            lastPosition = transform.position;
        }

        #region 模型
        public void SetModel(NamespaceID modelId)
        {
            if (Model)
            {
                Destroy(Model.gameObject);
                Model = null;
            }
            var model = Model.Create(modelId, transform, Entity.InitSeed);
            Model = model;
            if (!Model)
                return;
            UpdateEntityModel();
            UpdateArmorModel();
            Model.UpdateFrame(0);
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            if (Model)
            {
                Model.SetSimulationSpeed(simulationSpeed);
            }
        }
        public void ChangeArmorModel(NamespaceID modelID)
        {
            if (!Model)
                return;
            Model.RemoveArmor();
            Model.CreateArmor(modelID);
        }
        #endregion

        #region 更新
        public void UpdateFixed()
        {
            var posOffset = GetTransformOffset();

            var pos = Entity.Position;
            var currentTransPos = Level.LawnToTrans(pos);
            transform.position = Vector3.Lerp(lastPosition, currentTransPos + posOffset, 0.5f);

            UpdateShadow(posOffset);
            if (Model)
            {
                Model.UpdateFixed();
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            var posOffset = GetTransformOffset();

            var pos = Entity.Position;
            var currentTransPos = Level.LawnToTrans(pos);
            transform.position = Vector3.Lerp(lastPosition, currentTransPos + posOffset, 0.5f);
            lastPosition = transform.position;

            UpdateShadow(posOffset);
            if (Model)
            {
                UpdateEntityModel();
                UpdateArmorModel();
                Model.UpdateFrame(deltaTime);
            }
        }
        #endregion

        public void SetHovered(bool hovered, bool highlight)
        {
            isHovered = hovered;
            isHighlight = highlight;
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
        private void Update()
        {
            var engine = Entity.Level;
            bool cursorValid = isHovered && Level.IsGameRunning() && !engine.IsHoldingItem();
            if (cursorValid)
            {
                if (_cursorSource == null)
                {
                    _cursorSource = new EntityCursorSource(this, CursorType.Point);
                    Main.CursorManager.AddCursorSource(_cursorSource);
                }
            }
            else
            {
                if (_cursorSource != null)
                {
                    Main.CursorManager.RemoveCursorSource(_cursorSource);
                    _cursorSource = null;
                }
            }
        }
        private void OnDrawGizmos()
        {
            float pixelUnit = Level?.LawnToTransScale ?? 0.01f;

            foreach (var collider in Entity.GetEnabledColliders())
            {
                for (int h = 0; h < collider.GetHitboxCount(); h++)
                {
                    var hitbox = collider.GetHitbox(h);

                    Vector3 size = hitbox.GetBoundsSize() * pixelUnit;
                    var offset = hitbox.GetOffset();
                    var position = Entity.Position + offset;
                    float startX = position.x * pixelUnit;
                    float startY = position.y * pixelUnit;
                    float startZ = position.z * pixelUnit;
                    var collisionMask = Entity.CollisionMaskHostile | Entity.CollisionMaskFriendly;
                    Gizmos.color = new Color(
                        (collisionMask >> 0 & 7) / 7f,
                        (collisionMask >> 3 & 7) / 7f,
                        (collisionMask >> 6 & 3) / 3f, 1);
                    for (int i = 0; i < 12; i++)
                    {
                        int axe = i >> 2;
                        bool bit1 = (i & 1) != 0;
                        bool bit2 = (i & 2) != 0;
                        float offset1 = bit1 ? 0.5f : -0.5f;
                        float offset2 = bit2 ? 0.5f : -0.5f;
                        Vector3 dir = Vector3.right;

                        float x = startX, y = startY, z = startZ;

                        switch (axe)
                        {
                            // x-axis.
                            case 0:
                                x += -size.x * 0.5f;
                                y += size.y * (offset1 + 0.5f);
                                z += size.z * offset2;
                                dir = new Vector3(size.x, 0, 0);
                                break;
                            // y-axis.
                            case 1:
                                x += size.x * offset1;
                                y += 0;
                                z += size.z * offset2;
                                dir = new Vector3(0, size.y, 0);
                                break;
                            // z-axis.
                            case 2:
                                x += size.x * offset1;
                                y += size.y * (offset2 + 0.5f);
                                z += -size.z * 0.5f;
                                dir = new Vector3(0, 0, size.z);
                                break;
                        }
                        Vector3 start = new Vector3(x, z, -y);
                        Vector3 end = start + new Vector3(dir.x, dir.z, -dir.y);
                        Gizmos.DrawLine(start, end);
                    }

                    // Render Center.
                    Vector3 centerStart;
                    Vector3 centerEnd;
                    float centerLength = 0.05f;
                    centerStart = new Vector3(startX - centerLength, startZ, startY);
                    centerEnd = new Vector3(startX + centerLength, startZ, startY);
                    Gizmos.DrawLine(centerStart, centerEnd);

                    centerStart = new Vector3(startX, startZ - centerLength, startY);
                    centerEnd = new Vector3(startX, startZ + centerLength, startY);
                    Gizmos.DrawLine(centerStart, centerEnd);

                    centerStart = new Vector3(startX, startZ, startY - centerLength);
                    centerEnd = new Vector3(startX, startZ, startY + centerLength);
                    Gizmos.DrawLine(centerStart, centerEnd);
                }
            }
        }
        #endregion

        #region 事件回调
        private void PostInitCallback()
        {
            UpdateFrame(0);
        }
        private void OnChangeModelCallback(NamespaceID modelID)
        {
            SetModel(modelID);
        }
        private void OnArmorEquipCallback(Armor armor)
        {
            CreateArmorModel(armor);
        }
        private void OnArmorDestroyCallback(Armor armor, ArmorDamageResult result)
        {
        }
        private void OnArmorRemoveCallback(Armor armor)
        {
            RemoveArmorModel();
        }
        #endregion

        #region 接口实现
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this, eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this, eventData);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this, eventData);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data)
        {
            if (Entity.IsPreviewEnemy())
                return true;
            if (definition == null)
                return false;
            if (Entity.Type == EntityTypes.PICKUP)
            {
                if (!definition.IsForPickup())
                    return false;
            }
            else
            {
                if (!definition.IsForEntity())
                    return false;
            }
            var flags = definition.GetHeldFlagsOnEntity(Entity, data);
            return flags.HasFlag(HeldFlags.Valid);
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
            var shadowPos = Entity.Position;
            shadowPos.y = Entity.GetGroundHeight();
            shadowPos += Entity.GetShadowOffset();
            Shadow.transform.position = Level.LawnToTrans(shadowPos) + posOffset;
            float relativeHeight = Entity.GetRelativeY();
            float scale = 1 + relativeHeight / 300;
            float alpha = 1 - relativeHeight / 300;
            Shadow.transform.localScale = Entity.GetShadowScale() * scale;
            Shadow.gameObject.SetActive(!Entity.IsShadowHidden());
            Shadow.SetAlpha(Entity.GetShadowAlpha() * alpha);
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
        private void CreateArmorModel(Armor armor)
        {
            if (armor?.Definition == null)
                return;
            var modelID = armor.Definition.GetModelID();
            CreateArmorModel(modelID);
        }
        private void CreateArmorModel(NamespaceID modelID)
        {
            if (!Model)
                return;
            if (!NamespaceID.IsValid(modelID))
                return;
            Model.CreateArmor(modelID);
        }
        private void RemoveArmorModel()
        {
            if (!Model)
                return;
            Model.RemoveArmor();
        }
        private void UpdateArmorModel()
        {
            if (!Model)
                return;
            var armor = Entity.EquipedArmor;
            if (armor == null)
                return;
            var armorModel = Model.GetArmorModel();
            if (!armorModel)
                return;
            armorModel.RendererGroup.SetTint(armor.GetTint());
            armorModel.RendererGroup.SetColorOffset(armor.GetColorOffset());
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
            groundPos.y = Entity.GetGroundHeight();

            var rendererGroup = Model.RendererGroup;
            rendererGroup.SetTint(Entity.GetTint());
            rendererGroup.SetColorOffset(GetColorOffset());
            rendererGroup.SetGroundPosition(Level.LawnToTrans(groundPos));
            Model.CenterTransform.localEulerAngles = Entity.RenderRotation;
            Model.transform.localScale = Entity.GetDisplayScale();
            Model.RendererGroup.SortingLayerID = Entity.GetSortingLayer();
            Model.RendererGroup.SortingOrder = Entity.GetSortingOrder();

            var lightVisible = Entity.IsLightSource();
            var lightScaleLawn = Entity.GetLightRange();
            var lightScale = new Vector2(lightScaleLawn.x, Mathf.Max(lightScaleLawn.y, lightScaleLawn.z)) * Level.LawnToTransScale;
            var lightColor = Entity.GetLightColor();
            var randomLightScale = rng.Next(-0.05f, 0.05f);
            Model.RendererGroup.SetLight(lightVisible, lightScale, lightColor, Vector2.one * randomLightScale);

        }
        #endregion

        private Color GetColorOffset()
        {
            var color = Entity.GetColorOffset();
            if (isHighlight && Entity.Level.IsHoldingItem())
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
        public event Action<EntityController, PointerEventData> OnPointerEnter;
        public event Action<EntityController, PointerEventData> OnPointerExit;
        public event Action<EntityController, PointerEventData> OnPointerDown;
        #endregion

        #region 属性字段
        public static readonly Dictionary<int, float> zOffsetDict = new Dictionary<int, float>()
        {
            { EntityTypes.PLANT, 0 },
            { EntityTypes.OBSTACLE, 1 },
            { EntityTypes.BOSS, 2 },
            { EntityTypes.ENEMY, 3 },
            { EntityTypes.PROJECTILE, 4 },
            { EntityTypes.CART, 5 },
            { EntityTypes.EFFECT, 6 },
            { EntityTypes.PICKUP, 7 },
        };
        public MainManager Main => MainManager.Instance;
        public Model Model { get; private set; }
        public ShadowController Shadow => shadow;
        public Entity Entity { get; private set; }
        public LevelController Level { get; private set; }
        private RandomGenerator rng;
        private bool isHovered;
        private bool isHighlight;
        private EntityCursorSource _cursorSource;
        private Vector3 lastPosition;
        private IModelInterface bodyModelInterface;
        private IModelInterface armorModelInterface;
        [SerializeField]
        private ShadowController shadow;
        [SerializeField]
        private TooltipAnchor tooltipAnchor;
        #region shader相关属性
        protected MaterialPropertyBlock propertyBlock;
        #endregion shader相关属性

        TooltipAnchor ITooltipTarget.Anchor => tooltipAnchor;

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
            return target;
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
