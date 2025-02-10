using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.sapphire)]
    public class Sapphire : Gem
    {
        public Sapphire(string nsp, string name) : base(nsp, name)
        {
        }
        protected override bool CanMerge => true;
        protected override int MergeCount => 4;
        protected override NamespaceID MergeSource => VanillaPickupID.sapphire;
        protected override NamespaceID MergeTarget => VanillaPickupID.diamond;
    }
}