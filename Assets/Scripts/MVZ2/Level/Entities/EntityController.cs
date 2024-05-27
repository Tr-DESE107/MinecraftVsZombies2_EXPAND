using System;
using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public class EntityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        #region 公有方法
        public void Init(LevelController level, Entity entity)
        {
            this.level = level;
            this.Entity = entity;
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
        public void SetModel(NamespaceID modelId)
        {
            SetModel(CreateModel(modelId));
        }
        public void SetModel(Model model)
        {
            Model = model;
            if (!Model)
                return;
            Model.RendererGroup.SortingLayerID = SortingLayers.entities;
            UpdateEntityModel();
            UpdateArmorModel();
        }
        public void UpdateView(float deltaTime)
        {
            var nextPos = Entity.GetNextPosition();
            var pos = Entity.Pos;
            var posOffset = (nextPos.LawnToTrans() - pos.LawnToTrans()) * deltaTime / Time.fixedDeltaTime;
            float zOffset = 0;
            if (zOffsetDict.TryGetValue(Entity.Type, out float offset))
            {
                zOffset = offset * PositionHelper.LAWN_TO_TRANS_SCALE;
            }
            transform.position = pos.LawnToTrans() + posOffset + Vector3.back * zOffset;

            UpdateShadow(posOffset);
        }
        public void UpdateLogic(float deltaTime, float simulationSpeed)
        {
            if (Model)
            {
                UpdateEntityModel();
                UpdateArmorModel();
                Model.UpdateModel(deltaTime, simulationSpeed);
            }
        }
        public void SetHovered(bool hovered)
        {
            isHovered = hovered;
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void OnDrawGizmos()
        {
            float pixelUnit = PositionHelper.LAWN_TO_TRANS_SCALE;
            Vector3 size = Entity.GetScaledSize() * pixelUnit;
            var scaledBoundsOffset = Entity.GetScaledBoundsOffset();
            float
                startX = (Entity.Pos.x + scaledBoundsOffset.x) * pixelUnit,
                startY = (Entity.Pos.y + scaledBoundsOffset.y) * pixelUnit,
                startZ = (Entity.Pos.z + scaledBoundsOffset.z) * pixelUnit;
            Gizmos.color = new Color(
                ((Entity.CollisionMask >> 0) & 15) / 15f,
                ((Entity.CollisionMask >> 4) & 15) / 15f,
                ((Entity.CollisionMask >> 8) & 3) / 3f, 1);
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

        protected void UpdateShadow(Vector3 posOffset)
        {
            var shadowPos = Entity.Pos;
            shadowPos.y = Entity.GetGroundHeight();
            shadowPos += Entity.GetShadowOffset();
            Shadow.transform.position = shadowPos.LawnToTrans() + posOffset;
            float relativeHeight = Entity.GetRelativeY();
            float scale = 1 + relativeHeight / 300;
            float alpha = 1 - relativeHeight / 300;
            Shadow.transform.localScale = Entity.GetShadowScale() * scale;
            Shadow.gameObject.SetActive(!Entity.IsShadowHidden());
            Shadow.SetAlpha(Entity.GetShadowAlpha() * alpha);
        }
        private Model CreateModel(NamespaceID id)
        {
            var res = level.MainManager.ResourceManager;
            var modelTemplate = res.GetModel(id);
            if (modelTemplate == null)
                return null;
            return Instantiate(modelTemplate.gameObject, transform).GetComponent<Model>();
        }
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
            Model.RendererGroup.SetTint(Entity.GetTint());
            Model.RendererGroup.SetColorOffset(GetColorOffset());
            var groundPos = Entity.Pos;
            groundPos.y = Entity.GetGroundHeight();
            Model.RendererGroup.SetGroundPosition(groundPos.LawnToTrans());
            Model.CenterTransform.localEulerAngles = Entity.RenderRotation;
            Model.transform.localScale = Entity.RenderScale;
        }
        private Color GetColorOffset()
        {
            var color = Entity.GetColorOffset();
            if (isHovered && level.IsEntityValidForHeldItem(Entity))
            {
                color += new Color(0.5f, 0.5f, 0.5f, 0);
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
        public Model Model { get; private set; }
        public ShadowController Shadow => shadow;
        public Entity Entity { get; protected set; }
        protected LevelController level;
        [SerializeField]
        private ShadowController shadow;
        private bool isHovered;
        #region shader相关属性
        protected MaterialPropertyBlock propertyBlock;
        #endregion shader相关属性

        #endregion
    }
}
