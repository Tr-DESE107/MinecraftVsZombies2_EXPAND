namespace PVZEngine
{
    public class Boss : Entity
    {
        public Boss(Game level, int id, int seed) : base(level, id, seed)
        {
            SetFaction(Game.Option.RightFaction);
        }

        public override int Type => EntityTypes.BOSS;
    }
}