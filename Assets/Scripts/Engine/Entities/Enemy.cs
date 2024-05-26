using PVZEngine.Serialization;
using UnityEngine;

namespace PVZEngine
{
    public class Enemy : Entity
    {
        #region 公有方法
        #region 构造方法
        internal Enemy(Game level) : base(level)
        {
        }
        public Enemy(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
        }
        #endregion
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }
        #endregion
        protected override SerializableEntity CreateSerializableEntity() => new SerializableEntity();

        #region 属性字段
        public override int Type => EntityTypes.ENEMY;
        #endregion
    }
}