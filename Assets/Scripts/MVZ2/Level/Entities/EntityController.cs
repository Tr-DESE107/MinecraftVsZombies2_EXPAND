using System;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    public class EntityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        #region 公有方法
        public void Init(LevelController level, Entity entity, Model modelTemplate)
        {
            this.level = level;
            this.Entity = entity;
            entity.OnTriggerAnimation += (name) => { if (Model) Model.TriggerAnimator(name); };
            entity.OnSetAnimationBool += (name, value) => { if (Model) Model.SetAnimatorBool(name, value); };
            entity.OnSetAnimationInt += (name, value) => { if (Model) Model.SetAnimatorInt(name, value); };
            entity.OnSetAnimationFloat += (name, value) => { if (Model) Model.SetAnimatorFloat(name, value); };

            var model = Instantiate(modelTemplate.gameObject, transform).GetComponent<Model>();
            SetModel(model);
        }
        public void SetModel(Model model)
        {
            Model = model;
            Model.RendererGroup.SortingLayerID = SortingLayers.entities;
            UpdateEntityModelData();
        }
        public void UpdateView(float deltaTime)
        {
            var nextPos = Entity.GetNextPosition();
            var pos = Entity.Pos;
            var posOffset = (nextPos.LawnToTrans() - pos.LawnToTrans()) * deltaTime / Time.fixedDeltaTime;
            transform.position += posOffset;

            UpdateShadow(posOffset);
            UpdateEntityModelData();
            if (Model)
                Model.UpdateModel(deltaTime);
        }
        public void UpdateLogic()
        {
            if (Model)
                Model.UpdateLogic();
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

        #region 接口实现
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter?.Invoke(this);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit?.Invoke(this);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this);
        }
        #endregion

        protected void UpdateShadow(Vector3 posOffset)
        {
            var shadowPos = Entity.Pos;
            shadowPos.y = Entity.GetGroundHeight();
            shadowPos += Entity.ShadowOffset;
            Shadow.transform.position = shadowPos.LawnToTrans() + posOffset;
            float relativeHeight = Entity.GetRelativeY();
            float scale = 1 + relativeHeight / 300;
            float alpha = 1 - relativeHeight / 300;
            Shadow.transform.localScale = Entity.ShadowScale * scale;
            Shadow.gameObject.SetActive(Entity.ShadowVisible);
            Shadow.SetAlpha(Entity.ShadowAlpha * alpha);
        }
        private void UpdateEntityModelData()
        {
            Model.RendererGroup.SetTint(Entity.GetTint());
            Model.RendererGroup.SetColorOffset(GetColorOffset());
        }
        private Color GetColorOffset()
        {
            var color = Entity.GetColorOffset();
            if (isHovered)
            {
                color += new Color(0.5f, 0.5f, 0.5f);
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
        public event Action<EntityController> OnPointerEnter;
        public event Action<EntityController> OnPointerExit;
        public event Action<EntityController> OnPointerDown;
        #endregion

        #region 属性字段
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
