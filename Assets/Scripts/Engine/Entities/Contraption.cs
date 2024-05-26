using PVZEngine.Serialization;

namespace PVZEngine
{
    public class Contraption : Entity
    {
        #region 公有方法
        #region 构造方法
        internal Contraption(Game level) : base(level)
        {
        }
        public Contraption(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
        }
        #endregion
        protected override SerializableEntity CreateSerializableEntity() => new SerializableEntity();
        #endregion

        #region Properties
        public override int Type => EntityTypes.PLANT;
        #endregion
    }
}