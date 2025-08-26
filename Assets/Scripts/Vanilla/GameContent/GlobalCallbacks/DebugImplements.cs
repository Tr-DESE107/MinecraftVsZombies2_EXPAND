using MVZ2.Vanilla.Game;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Implements
{
    public class DebugImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LogicCallbacks.IS_SPECIAL_USER_NAME, IsSpecialUserNameCallback);
        }
        public void IsSpecialUserNameCallback(StringCallbackParams param, CallbackResult result)
        {
            var name = param.text;
            if (Global.Game.IsDebugUserName(name))
            {
                result.SetValue(true);
            }
        }
    }
}
