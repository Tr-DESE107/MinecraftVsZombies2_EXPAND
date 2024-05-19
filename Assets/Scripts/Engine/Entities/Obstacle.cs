namespace PVZEngine
{
    public sealed class Obstacle : Entity
    {
        public Obstacle(Game level, int id, int seed) : base(level, id, seed)
        {
            SetFaction(Game.Option.RightFaction);
            SetFriction(0.2f);
            SetFallDamage(22.5f);
        }
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }

        public override void Remove()
        {
            base.Remove();
            TakenGrid = null;
        }
        public override int Type => EntityTypes.OBSTACLE;
        public Grid TakenGrid { get; set; }
    }
}