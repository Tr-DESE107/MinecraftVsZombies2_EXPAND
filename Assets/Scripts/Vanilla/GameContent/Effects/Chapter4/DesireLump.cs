using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.desireLump)]
    public class DesireLump : EffectBehaviour
    {

        #region 公有方法
        public DesireLump(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStartPosition(entity, entity.Position);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (!parent.ExistsAndAlive())
            {
                entity.Remove();
                return;
            }
            var startPosition = GetStartPosition(entity);
            var t = 1 - entity.Timeout / (float)entity.GetMaxTimeout();
            var targetPosition = parent.GetCenter();
            var pos = Vector3.Lerp(startPosition, targetPosition, t);
            var maxY = Mathf.Max(targetPosition.y, 200);
            pos.y = MathTool.LerpParabolla(startPosition.y, targetPosition.y, maxY, t);
            entity.Position = pos;

            if (entity.Timeout <= 0)
            {
                parent.Spawn(VanillaPickupID.starshard, parent.GetCenter());
            }
        }
        public static Vector3 GetStartPosition(Entity entity) => entity.GetBehaviourField<Vector3>(PROP_START_POSITION);
        public static void SetStartPosition(Entity entity, Vector3 value) => entity.SetBehaviourField(PROP_START_POSITION, value);
        private static readonly VanillaEntityPropertyMeta PROP_START_POSITION = new VanillaEntityPropertyMeta("StartPosition");
    }
}