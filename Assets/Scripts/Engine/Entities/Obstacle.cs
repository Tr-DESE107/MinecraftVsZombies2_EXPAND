using PVZEngine.Serialization;

namespace PVZEngine
{
    public sealed class Obstacle : Entity
    {
        internal Obstacle(Game level) : base(level)
        {
        }
        public Obstacle(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
            SetFaction(Game.Option.RightFaction);
            SetFriction(0.2f);
            SetFallDamage(22.5f);
        }
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }
        public override int Type => EntityTypes.OBSTACLE;
        protected override SerializableEntity CreateSerializableEntity() => new SerializableEntity();
    }
}