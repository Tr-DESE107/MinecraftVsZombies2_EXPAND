using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Game;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Implements
{
    public class RandomChinaImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LogicCallbacks.IS_SPECIAL_USER_NAME, IsSpecialUserNameCallback);
            mod.AddTrigger(LogicCallbacks.GET_BLUEPRINT_SLOT_COUNT, GetBlueprintSlotCountCallback);
            mod.AddTrigger(LogicCallbacks.GET_INNATE_BLUEPRINTS, GetInnateBlueprintsCallback);
        }
        public void IsSpecialUserNameCallback(string name, TriggerResultBoolean result)
        {
            result.Result = Global.Game.IsRandomChinaUserName(name);
        }
        public void GetBlueprintSlotCountCallback(TriggerResultInt result)
        {
            if (!Global.Game.IsRandomChina())
                return;
            result.Result -= 2;
        }
        public void GetInnateBlueprintsCallback(TriggerResultNamespaceIDList result)
        {
            if (!Global.Game.IsRandomChina())
                return;
            result.Result.Add(VanillaContraptionID.randomChina);
        }
    }
}
