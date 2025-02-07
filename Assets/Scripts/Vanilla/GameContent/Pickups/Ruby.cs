using PVZEngine.Level;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.ruby)]
    public class Ruby : Gem
    {
        public Ruby(string nsp, string name) : base(nsp, name)
        {
        }
    }
}