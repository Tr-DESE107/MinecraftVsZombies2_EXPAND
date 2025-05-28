﻿using MVZ2Logic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class VanillaAreaProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<NamespaceID> MODEL_ID = Get<NamespaceID>("modelID");
        public static NamespaceID GetModelID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(MODEL_ID);
        }
        public static NamespaceID GetModelID(this AreaDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(MODEL_ID);
        }
        public static readonly PropertyMeta<SpriteReference> STARSHARD_ICON = Get<SpriteReference>("starshardIcon");
        public static SpriteReference GetStarshardIcon(this LevelEngine game)
        {
            return game.GetProperty<SpriteReference>(STARSHARD_ICON);
        }
        public static readonly PropertyMeta<Color> WATER_COLOR = Get<Color>("waterColor");
        public static readonly PropertyMeta<Color> WATER_COLOR_CENSORED = Get<Color>("waterColorCensored");
        public static Color GetWaterColorNormal(this LevelEngine game)
        {
            return game.GetProperty<Color>(WATER_COLOR);
        }
        public static Color GetWaterColorCensored(this LevelEngine game)
        {
            return game.GetProperty<Color>(WATER_COLOR_CENSORED);
        }
        public static Color GetWaterColor(this LevelEngine game)
        {
            return Global.HasBloodAndGore() ? game.GetWaterColorNormal() : game.GetWaterColorCensored();
        }

        public static readonly PropertyMeta<float> DOOR_Z = Get<float>("doorZ");
        public static readonly PropertyMeta<Color> BACKGROUND_LIGHT = Get<Color>("backgroundLight");
        public static readonly PropertyMeta<Color> GLOBAL_LIGHT = Get<Color>("globalLight");

        public static float GetDoorZ(this LevelEngine game)
        {
            return game.GetProperty<float>(DOOR_Z);
        }
        public static Color GetBackgroundLight(this LevelEngine level)
        {
            return level.GetProperty<Color>(BACKGROUND_LIGHT);
        }
        public static Color GetGlobalLight(this LevelEngine level)
        {
            return level.GetProperty<Color>(GLOBAL_LIGHT);
        }
    }
}

