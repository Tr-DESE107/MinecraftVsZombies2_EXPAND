using System;
using PVZEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2
{
    public class EntityController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region 公有方法
        public void SetEntity(Entity entity)
        {
            this.entity = entity;
            entity.OnTriggerAnimation += (name) => { if (View) View.TriggerAnimator(name); };
        }
        public void SetView(View view)
        {
            View before = View;
            View = view;
            View.RendererGroup.SortingLayerID = SortingLayers.entities;
        }
        #endregion

        #region 私有方法

        #region 生命周期
        private void OnDrawGizmos()
        {
            Vector3 size = entity.GetScaledSize();
            var boundsOffset = entity.GetScaledBoundsOffset();
            float
                x = entity.Pos.x + boundsOffset.x,
                y = entity.Pos.y + boundsOffset.y,
                z = entity.Pos.z + boundsOffset.z;
            for (int i = 0; i < 16; i++)
            {
                int axe = i >> 2;
                bool bit1 = (i & 1) != 0;
                bool bit2 = (i & 2) != 0;
                float offset1 = bit1 ? 0.5f : -0.5f;
                float offset2 = bit2 ? 0.5f : -0.5f;
                Vector3 dir = Vector3.right;


                Gizmos.color = Color.green;
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
                    case 3:
                        Gizmos.color = Color.cyan;
                        x += size.x * offset1;
                        y += size.y * (offset2 + 0.5f);
                        z += 0;
                        if (bit1)
                        {
                            if (bit2)
                                dir = new Vector3(0, -size.y, 0);
                            else
                                dir = new Vector3(-size.x, 0, 0);
                        }
                        else
                        {
                            if (bit2)
                                dir = new Vector3(size.x, 0, 0);
                            else
                                dir = new Vector3(0, size.y, 0);
                        }
                        break;
                }
                Vector3 start = new Vector3(x, z, y);
                Vector3 end = start + new Vector3(dir.x, dir.z, dir.y);
                Gizmos.DrawLine(start, end);
            }
        }
        private void Update()
        {
            var nextPos = entity.LawnRigid.GetNextPosition();
            var pos = entity.Pos;
            transform.position += (nextPos.LawnToTrans() - pos.LawnToTrans()) * Time.deltaTime / Time.fixedDeltaTime;
        }
        #endregion

        #region 接口实现
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {

        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {

        }
        #endregion

        protected void UpdateShadow()
        {
            var shadowPos = entity.Pos;
            shadowPos.y = entity.GetGroundHeight();
            shadowPos += entity.ShadowOffset;
            Shadow.transform.position = shadowPos.LawnToTrans();
            float relativeHeight = entity.GetRelativeY();
            float scale = 1 + relativeHeight / 3;
            float alpha = 1 - relativeHeight / 3;
            Shadow.transform.localScale = entity.ShadowScale * scale;
            Shadow.SetAlpha(entity.ShadowAlpha * alpha);
        }
        #endregion

        #region View
        public void TriggerView(string name)
        {
            if (View)
                View.TriggerAnimator(name);
        }
        public void SetViewBool(string name, bool value)
        {
            if (View)
                View.SetAnimatorBool(name, value);
        }
        public void SetViewInt(string name, int value)
        {
            if (View)
                View.SetAnimatorInt(name, value);
        }
        public void SetViewFloat(string name, float value)
        {
            if (View)
                View.SetAnimatorFloat(name, value);
        }
        #endregion

        #region 属性字段
        public View View { get; private set; }
        public ShadowController Shadow => shadow;
        protected Entity entity;
        [SerializeField]
        private ShadowController shadow;
        #region shader相关属性
        protected MaterialPropertyBlock propertyBlock;
        #endregion shader相关属性

        #endregion
    }
}
