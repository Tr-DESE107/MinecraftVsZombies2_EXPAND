using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Buffs.SeedPacks
{
    [BuffDefinition(VanillaBuffNames.SeedPack.slendermanMindSwap)]
    public class SlenderManMindSwapBuff : BuffDefinition
    {
        public SlenderManMindSwapBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.mindSwap, VanillaModelID.mindSwap);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, 30);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);

            if (timeout == 15)
            {
                TransformBlueprint(buff);
            }
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        private void TransformBlueprint(Buff buff)
        {
            SeedPack seedPack = buff.GetSeedPack();
            if (seedPack == null)
                return;
            var targetID = buff.GetProperty<NamespaceID>(PROP_TARGET_ID);
            if (!NamespaceID.IsValid(targetID))
                return;
            var targetDefinition = buff.Level.Content.GetSeedDefinition(targetID);
            if (targetDefinition == null)
                return;
            seedPack.ChangeDefinition(targetDefinition);
            var drawn = seedPack.GetDrawnConveyorSeed();
            if (NamespaceID.IsValid(drawn))
            {
                buff.Level.PutSeedToConveyorPool(drawn);
                seedPack.SetDrawnConveyorSeed(null);
            }
            if (buff.Level.IsHoldingBlueprint(seedPack))
            {
                buff.Level.ResetHeldItem();
            }
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public static readonly VanillaBuffPropertyMeta PROP_TARGET_ID = new VanillaBuffPropertyMeta("TargetID");
    }
}
