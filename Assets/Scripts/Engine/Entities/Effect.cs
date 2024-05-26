using PVZEngine.Serialization;

namespace PVZEngine
{
    public sealed class Effect : Entity
    {
        internal Effect(Game level) : base(level)
        {
        }
        public Effect(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
            ShadowVisible = false;
        }
        protected override SerializableEntity CreateSerializableEntity() => new SerializableEntity();

        public override int Type => EntityTypes.EFFECT;
    }
}