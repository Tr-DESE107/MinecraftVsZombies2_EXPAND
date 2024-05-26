using PVZEngine.Serialization;

namespace PVZEngine
{
    public class Boss : Entity
    {
        internal Boss(Game level) : base(level)
        {
        }
        public Boss(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
            SetFaction(Game.Option.RightFaction);
            SetFriction(0.15f);
        }


        public override bool IsFacingLeft()
        {
            return !FlipX;
        }
        protected override SerializableEntity CreateSerializableEntity() => new SerializableEntity();

        public override int Type => EntityTypes.BOSS;
    }
}