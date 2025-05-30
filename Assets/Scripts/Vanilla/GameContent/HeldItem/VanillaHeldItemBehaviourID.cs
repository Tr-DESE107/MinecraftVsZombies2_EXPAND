using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.HeldItems
{
    public static class VanillaHeldItemBehaviourNames
    {
        public const string pickaxe = "pickaxe";
        public const string starshard = "starshard";
        public const string trigger = "trigger";
        public const string sword = "sword";
        public const string forcePad = "force_pad";
        public const string breakoutBoard = "breakout_board";
        public const string blueprintPickup = "blueprint_pickup";
        public const string classicBlueprint = "classic_blueprint";
        public const string conveyorBlueprint = "conveyor_blueprint";

        public const string rightMouseCancel = "right_mouse_cancel";
        public const string triggerCart = "triggerCart";
        public const string pickup = "pickup";
        public const string selectBlueprint = "select_blueprint";
    }
    public static class VanillaHeldItemBehaviourID
    {
        public static readonly NamespaceID pickaxe = Get(VanillaHeldItemBehaviourNames.pickaxe);
        public static readonly NamespaceID starshard = Get(VanillaHeldItemBehaviourNames.starshard);
        public static readonly NamespaceID trigger = Get(VanillaHeldItemBehaviourNames.trigger);
        public static readonly NamespaceID sword = Get(VanillaHeldItemBehaviourNames.sword);
        public static readonly NamespaceID forcePad = Get(VanillaHeldItemBehaviourNames.forcePad);
        public static readonly NamespaceID breakoutBoard = Get(VanillaHeldItemBehaviourNames.breakoutBoard);
        public static readonly NamespaceID blueprintPickup = Get(VanillaHeldItemBehaviourNames.blueprintPickup);
        public static readonly NamespaceID classicBlueprint = Get(VanillaHeldItemBehaviourNames.classicBlueprint);
        public static readonly NamespaceID conveyorBlueprint = Get(VanillaHeldItemBehaviourNames.conveyorBlueprint);

        public static readonly NamespaceID rightMouseCancel = Get(VanillaHeldItemBehaviourNames.rightMouseCancel);
        public static readonly NamespaceID triggerCart = Get(VanillaHeldItemBehaviourNames.triggerCart);
        public static readonly NamespaceID pickup = Get(VanillaHeldItemBehaviourNames.pickup);
        public static readonly NamespaceID selectBlueprint = Get(VanillaHeldItemBehaviourNames.selectBlueprint);
        public static NamespaceID Get(string name)
        {
            return new NamespaceID(VanillaMod.spaceName, name);
        }
    }
}
