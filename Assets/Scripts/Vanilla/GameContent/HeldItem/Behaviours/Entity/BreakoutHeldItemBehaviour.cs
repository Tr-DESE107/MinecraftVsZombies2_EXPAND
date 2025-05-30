using MVZ2.GameContent.Effects;
using MVZ2.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.breakoutBoard)]
    public class BreakoutBoardHeldItemBehaviour : EntityHeldItemBehaviour
    {
        public BreakoutBoardHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return target is HeldItemTargetLawn targetLawn && targetLawn.Area == LawnArea.Main;
        }

        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.None;
        }

        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var entity = GetEntity(target, data);
            if (pointerParams.IsInvalidClickButton() || pointerParams.IsInvalidReleaseAction())
                return;
            BreakoutBoard.FirePearl(entity);
        }
    }
}
