namespace PVZEngine
{
    public sealed class Effect : Entity
    {
        public Effect(Game level, int id, int seed) : base(level, id, seed)
        {
            ShadowVisible = false;
        }

        public override int Type => EntityTypes.EFFECT;
    }
}