using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.ruby)]
    public class Ruby : Gem
    {
        public Ruby(string nsp, string name) : base(nsp, name)
        {
        }
        protected override bool CanMerge => true;
        protected override int MergeCount => 4;
        protected override NamespaceID MergeSource => VanillaPickupID.ruby;
        protected override NamespaceID MergeTarget => VanillaPickupID.sapphire;
    }
}