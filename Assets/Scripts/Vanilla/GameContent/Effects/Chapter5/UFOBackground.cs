using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.ufoBackground)]
    public class UFOBackground : EffectBehaviour
    {
        #region 公有方法
        public UFOBackground(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion

        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationInt("Type", GetType(entity));
        }
        public static int GetType(Entity entity) => entity.GetBehaviourField<int>(PROP_TYPE);
        public static void SetType(Entity entity, int value) => entity.SetBehaviourField(PROP_TYPE, value);
        public const float MIN_X = 520;
        public const float MAX_X = 720;
        public const float MIN_Y = -160;
        public const float MAX_Y = -120;
        public const float MIN_Z = 40;
        public const float MAX_Z = 80;
        public const float MAX_SPEED = MIN_SPEED * 1.25f;
        public const float MIN_SPEED = (600 - (MIN_Y + MIN_Z)) / TIMEOUT;
        public const int TIMEOUT = 120;
        public static readonly Vector3 FLY_DIRECTION = new Vector3(0.2f, 0, 1);

        private static readonly VanillaEntityPropertyMeta<int> PROP_TYPE = new VanillaEntityPropertyMeta<int>("type");
    }
}