using MVZ2.Level.Components;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.Level
{
    public class ClassicBlueprintController : RuntimeBlueprintController
    {
        public ClassicBlueprintController(ILevelController level, Blueprint ui, int index, SeedPack seedPack) : base(level, ui, index, seedPack)
        {
        }
        protected override void OnDestroy()
        {
            Controller.BlueprintController.DestroyClassicBlueprintAt(Index);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var maxCharge = SeedPack.GetMaxRecharge();
            ui.SetRecharge(maxCharge == 0 ? 0 : 1 - SeedPack.GetRecharge() / maxCharge);
            ui.SetDisabled(!CanPick());
            ui.SetTwinkling(SeedPack.IsTwinkling() || (Level.IsHoldingTrigger() && SeedPack.CanInstantTrigger()));
            ui.SetSelected(Level.IsHoldingClassicBlueprint(Index));
        }
        public override bool CanPick(out string errorMessage)
        {
            var seed = SeedPack;
            if (seed == null)
            {
                errorMessage = null;
                return false;
            }
            if (!seed.IsCharged())
            {
                errorMessage = Vanilla.VanillaStrings.TOOLTIP_RECHARGING;
                return false;
            }
            if (seed.Level.Energy < seed.GetCost())
            {
                errorMessage = Vanilla.VanillaStrings.TOOLTIP_NOT_ENOUGH_ENERGY;
                return false;
            }
            if (seed.IsDisabled())
            {
                errorMessage = seed.GetDisableMessage();
                return false;
            }
            errorMessage = null;
            return true;
        }
        protected override void OnPickup(bool instantTrigger)
        {
            var blueprint = SeedPack;
            var blueprintDef = blueprint.Definition;
            var seedType = blueprintDef.GetSeedType();
            if (seedType == SeedTypes.ENTITY)
            {
                // 设置当前手持物品。
                Level.SetHeldItem(new HeldItemStruct()
                {
                    Type = BuiltinHeldTypes.blueprint,
                    ID = Index,
                    InstantTrigger = instantTrigger,
                    Priority = 0,
                });
                Level.PlaySound(VanillaSoundID.pick);
            }
            else if (seedType == SeedTypes.OPTION)
            {
                var optionID = blueprintDef.GetSeedOptionID();
                var optionDef = Level.Content.GetSeedOptionDefinition(optionID);
                if (optionDef == null)
                    return;
                optionDef.Use(blueprint);
                Level.AddEnergy(-blueprint.GetCost());
            }
        }
        protected override SerializableBlueprintController CreateSerializable()
        {
            return new SerializableClassicBlueprintController()
            {
            };
        }
        protected override void LoadSerializable(SerializableBlueprintController serializable)
        {
        }
    }
    public class SerializableClassicBlueprintController : SerializableBlueprintController
    {
    }
}
