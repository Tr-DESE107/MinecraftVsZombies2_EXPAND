namespace PVZEngine
{
    public class Boss : Entity
    {
        public Boss(Game level, int id, int seed) : base(level, id, seed)
        {
            SetFaction(Game.Option.RightFaction);
            SetFriction(0.15f);
        }
        public override bool IsFacingLeft()
        {
            return !FlipX;
        }

        public override int Type => EntityTypes.BOSS;
    }
}