using MVZ2.Level.Components;
using MVZ2.UI;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Level
{
    public class ConveyorBlueprintController : RuntimeBlueprintController
    {
        public ConveyorBlueprintController(ILevelController level, Blueprint ui, int index, SeedPack seedPack) : base(level, ui, index, seedPack)
        {
            Position = level.GetEngine().GetConveyorSlotCount();
        }
        protected override void OnDestroy()
        {
            Controller.BlueprintController.DestroyConveyorBlueprintAt(Index);
        }
        public override BlueprintViewData GetBlueprintViewData()
        {
            BlueprintViewData viewData = Main.ResourceManager.GetBlueprintViewData(SeedPack);
            viewData.cost = string.Empty;
            return viewData;
        }
        public override void UpdateFixed()
        {
            base.UpdateFixed();
            var index = Index;
            Position -= Controller.BlueprintController.GetConveyorSpeed() / 45f;
            float minPosition = 0;
            if (index > 0)
            {
                var prevSeed = Controller.BlueprintController.GetConveyorBlueprintController(index - 1);
                minPosition = prevSeed.Position + 1;
            }
            Position = Mathf.Max(Position, minPosition);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            ui.SetRecharge(0);
            ui.SetDisabled(false);
            ui.SetTwinkling(SeedPack.IsTwinkling() || (Level.IsHoldingTrigger() && SeedPack.CanInstantTrigger()));
            ui.SetSelected(Level.IsHoldingConveyorBlueprint(Index));

            Controller.BlueprintController.SetConveyorBlueprintUIPosition(Index, Position);
        }
        public override bool CanPick(out string errorMessage)
        {
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
                    Type = BuiltinHeldTypes.conveyor,
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
            }
        }
        public SerializableConveyorBlueprintController ToSerializable()
        {
            return new SerializableConveyorBlueprintController()
            {
                position = Position
            };
        }
        public void LoadFromSerializable(SerializableConveyorBlueprintController serializable)
        {
            Position = serializable.position;
        }
        public float Position { get; set; }
    }
    public class SerializableConveyorBlueprintController
    {
        public float position;
    }
}
