using MVZ2.GameContent;
using MVZ2.GameContent.Seeds;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaLevel
    {
        private static float GetHalloweenGroundHeight(float x)
        {
            if (x < 185)
            {
                // 地面和第一层的交界处
                if (x > 175)
                {
                    return Mathf.Lerp(0, 48, (185 - x) * 10);
                }
                else if (x > 140)
                {
                    return 48;
                }
                // 第一层和第二层的交界处
                else if (x > 130)
                {
                    return Mathf.Lerp(48, 96, (140 - x) * 10);
                }
                else if (x > 95)
                {
                    return 96;
                }
                // 第二层和第三层的交界处
                else if (x > 85)
                {
                    return Mathf.Lerp(96, 144, (95 - x) * 10);
                }
                else
                {
                    return 144;
                }
            }
            return 0;
        }
        public static NamespaceID GetHeldEntityID(this LevelEngine level)
        {
            if (level.GetHeldItemType() != HeldTypes.blueprint)
                return null;
            var seed = level.GetSeedPackAt(level.GetHeldItemID());
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return null;
            return seedDef.GetSeedEntityID();
        }
    }
}
