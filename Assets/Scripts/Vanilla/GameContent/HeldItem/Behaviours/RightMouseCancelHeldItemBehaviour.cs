using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.rightMouseCancel)]
    public class RightMouseCancelHeldItemBehaviour : HeldItemBehaviourDefinition
    {
        public RightMouseCancelHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaCallbacks.POST_POINTER_ACTION, PostPointerActionCallback, filter: PointerPhase.Press);
        }
        private void PostPointerActionCallback(VanillaCallbacks.PostPointerActionParams param, CallbackResult result)
        {
            var type = param.type;
            var button = param.button;
            if (type != PointerTypes.MOUSE || button != MouseButtons.RIGHT)
                return;
            var level = Global.Level.GetLevel();
            if (level == null || !level.IsGameRunning())
                return;
            var heldItemDef = level.GetHeldItemDefinition();
            if (heldItemDef == null)
                return;
            if (!heldItemDef.HasBehaviour(level, level.GetHeldItemData(), this))
                return;
            if (level.CancelHeldItem())
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
    }
}
