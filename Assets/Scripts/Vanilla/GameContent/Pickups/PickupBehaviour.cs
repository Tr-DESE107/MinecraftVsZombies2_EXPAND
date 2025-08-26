using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.pickupAutoCollect)]
    public class PickupAutoCollect : EntityBehaviourDefinition
    {
        public PickupAutoCollect(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            if (!pickup.IsCollected() && !pickup.NoCollect())
            {
                var level = pickup.Level;
                var willAutoCollect = false;
                if (level.IsCleared)
                {
                    willAutoCollect = true;
                }
                else if (level.IsAutoCollectAll())
                {
                    willAutoCollect = true;
                }
                else if (pickup.GetEnergyValue() > 0 && level.IsAutoCollectEnergy())
                {
                    willAutoCollect = true;
                }
                else if (pickup.GetMoneyValue() > 0 && level.IsAutoCollectMoney())
                {
                    willAutoCollect = true;
                }
                else if (pickup.IsEntityOf(VanillaPickupID.starshard) && level.IsAutoCollectStarshard())
                {
                    willAutoCollect = true;
                }
                if (willAutoCollect && pickup.CanAutoCollect() && pickup.CanCollect())
                {
                    pickup.Collect();
                }
            }
        }
    }
}
