using System;
using System.Collections.Generic;
using MVZ2.Cursors;
using MVZ2.Level;
using MVZ2.Managers;
using MVZ2.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Entities;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Entities
{
    public class EntityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        #region 公有方法
        public void Init(LevelController level, Entity entity)
        {
            Level = level;
            Entity = entity;
            gameObject.name = entity.Definition.GetID().ToString();
            entity.PostInit += PostInitCallback;
            entity.OnTriggerAnimation += OnTriggerAnimationCallback;
            entity.OnSetAnimationBool += OnSetAnimationBoolCallback;
            entity.OnSetAnimationInt += OnSetAnimationIntCallback;
            entity.OnSetAnimationFloat += OnSetAnimationFloatCallback;

            entity.OnEquipArmor += OnArmorEquipCallback;
            entity.OnDestroyArmor += OnArmorDestroyCallback;
            entity.OnRemoveArmor += OnArmorRemoveCallback;

            entity.OnModelChanged += OnModelChangedCallback;
            entity.OnSetModelProperty += OnSetModelPropertyCallback;
            SetModel(Entity.ModelID);
        }
        #region 模型
        public void SetModel(NamespaceID modelId)
        {
            SetModel(CreateModel(modelId));
        }
        public void SetModel(Model model)
        {
            if (Model)
            {
                Destroy(Model.gameObject);
                Model = null;
            }
            Model = model;
            if (!Model)
                return;
            UpdateEntityModel();
            UpdateArmorModel();
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
            movementTransitionTime = 0;
            if (Model)
            {
                Model.UpdateFixed();
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            movementTransitionTime += deltaTime;
            var posOffset = GetTransformOffset();
            UpdateTransform(posOffset);
            UpdateShadow(posOffset);
            if (Model)
            {
                UpdateEntityModel();
                UpdateArmorModel();
                Model.UpdateFrame(deltaTime);
            }
        }
        #endregion

        public void SetHovered(bool hovered)
        {
            isHovered = hovered;
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
            bool cursorValid = isHovered && Level.IsGameRunning() && !engine.IsHoldingItem() && engine.GetHeldFlagsOnEntity(Entity).HasFlag(HeldFlags.Valid);
            if (cursorValid)
            {
                if (_cursorSource == null)
                {
                    _cursorSource = new EntityCursorSource(this, CursorType.Point);
                    CursorManager.AddSource(_cursorSource);
                }
            }
            else
            {
                if (_cursorSource != null)
                {
                    CursorManager.RemoveSource(_cursorSource);
                    _cursorSource = null;
                }
            }
        }
        private void OnDrawGizmos()
        {
            float pixelUnit = Level?.LawnToTransScale ?? 0.01f;
            Vector3 size = Entity.GetScaledSize() * pixelUnit;
            var scaledBoundsOffset = Entity.GetScaledBoundsOffset();
            float
                startX = (Entity.Position.x + scaledBoundsOffset.x) * pixelUnit,
                startY = (Entity.Position.y + scaledBoundsOffset.y) * pixelUnit,
                startZ = (Entity.Position.z + scaledBoundsOffset.z) * pixelUnit;
            Gizmos.color = new Color(
                (Entity.CollisionMask >> 0 & 7) / 7f,
                (Entity.CollisionMask >> 3 & 7) / 7f,
                (Entity.CollisionMask >> 6 & 3) / 3f, 1);
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
                    case 0:
                        x += -size.x * 0.5f;
                        y += size.y * (offset1 + 0.5f);
                        z += size.z * offset2;
                        dir = new Vector3(size.x, 0, 0);
                        break;
                    case 1:
                        x += size.x * offset1;
                        y += 0;
                        z += size.z * offset2;
                        dir = new Vector3(0, size.y, 0);
                        break;
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
        #endregion

        #region 事件回调
        private void PostInitCallback()
        {
            UpdateFrame(0);
        }
        private void OnTriggerAnimationCallback(string name, EntityAnimationTarget target)
        {
            var targetModel = GetAnimationTargetModel(target);
            if (!targetModel)
                return;
            targetModel.TriggerAnimator(name);
        }
        private void OnSetAnimationBoolCallback(string name, EntityAnimationTarget target, bool value)
        {
            var targetModel = GetAnimationTargetModel(target);
            if (!targetModel)
                return;
            targetModel.SetAnimatorBool(name, value);
        }
        private void OnSetAnimationIntCallback(string name, EntityAnimationTarget target, int value)
        {
            var targetModel = GetAnimationTargetModel(target);
            if (!targetModel)
                return;
            targetModel.SetAnimatorInt(name, value);
        }
        private void OnSetAnimationFloatCallback(string name, EntityAnimationTarget target, float value)
        {
            var targetModel = GetAnimationTargetModel(target);
            if (!targetModel)
                return;
            targetModel.SetAnimatorFloat(name, value);
        }
        private void OnArmorEquipCallback(Armor armor)
        {
            CreateArmorModel(armor);
        }
        private void OnArmorDestroyCallback(Armor armor, DamageResult result)
        {
        }
        private void OnArmorRemoveCallback(Armor armor)
        {
            RemoveArmorModel();
        }
        private void OnModelChangedCallback(NamespaceID modelID)
        {
            SetModel(modelID);
        }
        private void OnSetModelPropertyCallback(string name, object value)
        {
            if (!Model)
                return;
            Model.SetProperty(name, value);
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
        #endregion

        #region 位置
        protected void UpdateTransform(Vector3 posOffset)
        {
            var pos = Entity.Position;
            var currentTransPos = Level.LawnToTrans(pos);
            transform.position = currentTransPos + posOffset;
        }
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
        protected Vector3 GetTransitionOffset()
        {
            var pos = Entity.Position;
            var nextPos = Entity.GetNextPosition();
            var nextTransPos = Level.LawnToTrans(nextPos);
            var currentTransPos = Level.LawnToTrans(pos);

            var passedTime = movementTransitionTime;
            var passedTicks = passedTime * Entity.Level.TPS;
            var targetTransPos = Vector3.Lerp(currentTransPos, nextTransPos, passedTicks);
            var transitionOffset = targetTransPos - currentTransPos;
            return transitionOffset;
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
            var transitionOffset = GetTransitionOffset();
            float zOffset = GetZOffset();
            return transitionOffset + Vector3.back * zOffset;
        }
        #endregion

        #region 护甲
        private void CreateArmorModel(Armor armor)
        {
            if (!Model)
                return;
            Model.SetArmor(CreateModel(armor.Definition.GetModelID()));
        }
        private void RemoveArmorModel()
        {
            if (!Model)
                return;
            Model.RemoveArmor();
        }
        private void UpdateArmorModel()
        {
            if (Entity.EquipedArmor == null)
            {
                RemoveArmorModel();
                return;
            }
            if (!Model.ArmorModel)
            {
                CreateArmorModel(Entity.EquipedArmor);
            }
            UpdateArmorModelProperties(Entity.EquipedArmor);
        }
        private void UpdateArmorModelProperties(Armor armor)
        {
            Model.ArmorModel.RendererGroup.SetTint(armor.GetTint());
            Model.ArmorModel.RendererGroup.SetColorOffset(armor.GetColorOffset());
        }
        #endregion

        #region 模型
        private Model CreateModel(NamespaceID id)
        {
            var res = Main.ResourceManager;
            var modelMeta = res.GetModelMeta(id);
            if (modelMeta == null)
                return null;
            var modelTemplate = res.GetModel(modelMeta.Path);
            if (modelTemplate == null)
                return null;
            return Instantiate(modelTemplate.gameObject, transform).GetComponent<Model>();
        }
        private Model GetAnimationTargetModel(EntityAnimationTarget target)
        {
            switch (target)
            {
                case EntityAnimationTarget.Entity:
                    return Model;
                case EntityAnimationTarget.Armor:
                    return Model.ArmorModel;
            }
            return null;
        }
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
            Model.transform.localScale = Entity.RenderScale;
            Model.RendererGroup.SortingLayerID = Entity.GetSortingLayer();
            Model.RendererGroup.SortingOrder = Entity.GetSortingOrder();

            var lightVisible = Entity.IsLightSource();
            var lightScaleLawn = Entity.GetLightRange();
            var lightScale = new Vector2(lightScaleLawn.x, Mathf.Max(lightScaleLawn.y, lightScaleLawn.z)) * Level.LawnToTransScale;
            var lightColor = Entity.GetLightColor();
            var randomLightScale = Level.RNG.Next(-0.05f, 0.05f);
            Model.RendererGroup.SetLight(lightVisible, lightScale, lightColor, Vector2.one * randomLightScale);

        }
        #endregion

        private Color GetColorOffset()
        {
            var color = Entity.GetColorOffset();
            if (isHovered && Entity.Level.IsHoldingItem() && Entity.Level.GetHeldFlagsOnEntity(Entity).HasFlag(HeldFlags.Valid))
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
        private bool isHovered;
        private EntityCursorSource _cursorSource;
        private float movementTransitionTime;
        #region shader相关属性
        protected MaterialPropertyBlock propertyBlock;
        [SerializeField]
        private ShadowController shadow;
        #endregion shader相关属性

        #endregion
    }
    public class EntityCursorSource : ICursorSource
    {
        public EntityCursorSource(EntityController target, CursorType type, int priority = 0)
        {
            this.target = target;
            this.type = type;
            this.priority = priority;
        }

        public bool IsValid()
        {
            return target;
        }

        public EntityController target;
        private int priority;
        public int Priority => priority;
        private CursorType type;
        public CursorType CursorType => type;
    }
    public class SerializableEntityController
    {
        public long id;
        public SerializableModelData model;
    }
}
