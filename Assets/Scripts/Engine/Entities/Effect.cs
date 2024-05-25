namespace PVZEngine
{
    public sealed class Effect : Entity
    {
        public Effect(Game level, int id, EntityDefinition definition, int seed) : base(level, id, definition, seed)
        {
            ShadowVisible = false;
        }

        public override int Type => EntityTypes.EFFECT;
    }
}