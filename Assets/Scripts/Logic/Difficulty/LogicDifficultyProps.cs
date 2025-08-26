using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2Logic.Difficulties
{
    [PropertyRegistryRegion(PropertyRegions.difficulty)]
    public static class LogicDifficultyProps
    {
        public static PropertyMeta<T> Get<T>(string name, T defaultValue = default)
        {
            return new PropertyMeta<T>(name, defaultValue);
        }
        public static readonly PropertyMeta<string> NAME = Get<string>("name");
        public static string GetName(this DifficultyDefinition def) => def.GetProperty<string>(NAME);
        public static readonly PropertyMeta<int> VALUE = Get<int>("value");
        public static int GetValue(this DifficultyDefinition def) => def.GetProperty<int>(VALUE);
        public static readonly PropertyMeta<int> CLEAR_MONEY = Get<int>("clear_money");
        public static int GetClearMoney(this DifficultyDefinition def) => def.GetProperty<int>(CLEAR_MONEY);
        public static readonly PropertyMeta<int> CART_CONVERT_MONEY = Get<int>("cart_convert_money");
        public static int GetCartConvertMoney(this DifficultyDefinition def) => def.GetProperty<int>(CART_CONVERT_MONEY);
        public static readonly PropertyMeta<int> RERUN_CLEAR_MONEY = Get<int>("rerun_clear_money");
        public static int GetRerunClearMoney(this DifficultyDefinition def) => def.GetProperty<int>(RERUN_CLEAR_MONEY);

        public static readonly PropertyMeta<int> PUZZLE_MONEY = Get<int>("puzzle_money");
        public static int GetPuzzleMoney(this DifficultyDefinition def) => def.GetProperty<int>(PUZZLE_MONEY);

        public static readonly PropertyMeta<NamespaceID> BUFF_ID = Get<NamespaceID>("buff_id");
        public static NamespaceID GetBuffID(this DifficultyDefinition def) => def.GetProperty<NamespaceID>(BUFF_ID);
        public static readonly PropertyMeta<NamespaceID> I_ZOMBIE_BUFF_ID = Get<NamespaceID>("i_zombie_buff_id");
        public static NamespaceID GetIZombieBuffID(this DifficultyDefinition def) => def.GetProperty<NamespaceID>(I_ZOMBIE_BUFF_ID);

        public static readonly PropertyMeta<SpriteReference> MAP_BUTTON_BORDER_BACK = Get<SpriteReference>("map_button_border_back");
        public static SpriteReference GetMapButtonBorderBack(this DifficultyDefinition def) => def.GetProperty<SpriteReference>(MAP_BUTTON_BORDER_BACK);
        public static readonly PropertyMeta<SpriteReference> MAP_BUTTON_BORDER_BOTTOM = Get<SpriteReference>("map_button_border_bottom");
        public static SpriteReference GetMapButtonBorderBottom(this DifficultyDefinition def) => def.GetProperty<SpriteReference>(MAP_BUTTON_BORDER_BOTTOM);
        public static readonly PropertyMeta<SpriteReference> MAP_BUTTON_BORDER_OVERLAY = Get<SpriteReference>("map_button_border_overlay");
        public static SpriteReference GetMapButtonBorderOverlay(this DifficultyDefinition def) => def.GetProperty<SpriteReference>(MAP_BUTTON_BORDER_OVERLAY);
        public static readonly PropertyMeta<SpriteReference> ARCADE_ICON = Get<SpriteReference>("arcade_icon");
        public static SpriteReference GetArcadeIcon(this DifficultyDefinition def) => def.GetProperty<SpriteReference>(ARCADE_ICON);
    }
}