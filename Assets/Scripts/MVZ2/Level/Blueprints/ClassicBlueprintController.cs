﻿using MVZ2.UI;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
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
            CurrentRecharged = false;
        }
        public override void UpdateFixed()
        {
            base.UpdateFixed();
            var maxRecharge = SeedPack.GetMaxRecharge();
            var recharge = SeedPack.GetRecharge();
            bool recharged = recharge >= maxRecharge;
            if (recharged != CurrentRecharged)
            {
                CurrentRecharged = recharged;
                if (recharged)
                {
                    ui.RechargeFlash();
                }
            }
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);

            var maxRecharge = SeedPack.GetMaxRecharge();
            var recharge = SeedPack.GetRecharge();
            ui.SetRecharge(maxRecharge == 0 ? 0 : 1 - recharge / maxRecharge);
            ui.SetDisabled(!CanPick());
            ui.SetTwinkleAlpha(ShouldBlueprintTwinkle(SeedPack) ? Controller.GetTwinkleAlpha() : 0);
            ui.SetSelected(Level.IsHoldingClassicBlueprint(Index));
            ui.UpdateAnimation(deltaTime);
        }
        public override bool IsCommandBlock()
        {
            return SeedPack.IsCommandBlock();
        }
        protected override SerializableBlueprintController CreateSerializable()
        {
            return new SerializableClassicBlueprintController()
            {
                recharged = CurrentRecharged
            };
        }
        protected override void LoadSerializable(SerializableBlueprintController serializable)
        {
            if (serializable is not SerializableClassicBlueprintController seri)
                return;
            CurrentRecharged = seri.recharged;
        }
        public bool CurrentRecharged { get; private set; }
    }
    public class SerializableClassicBlueprintController : SerializableBlueprintController
    {
        public bool recharged;
    }
}
