﻿using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Game;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;

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
        public void IsSpecialUserNameCallback(StringCallbackParams param, CallbackResult result)
        {
            var name = param.text;
            if (Global.Game.IsRandomChinaUserName(name))
            {
                result.SetValue(true);
            }
        }
        public void GetBlueprintSlotCountCallback(EmptyCallbackParams param, CallbackResult result)
        {
            if (!Global.Game.IsRandomChina())
                return;
            var value = result.GetValue<int>();
            result.SetValue(value - 2);
        }
        public void GetInnateBlueprintsCallback(LogicCallbacks.GetInnateBlueprintsParams param, CallbackResult result)
        {
            if (!Global.Game.IsRandomChina())
                return;
            param.list.Add(VanillaContraptionID.randomChina);
        }
    }
}
