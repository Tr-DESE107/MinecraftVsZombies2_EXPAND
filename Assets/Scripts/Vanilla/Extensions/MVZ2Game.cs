using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Game
    {
        public static Vector2 GetEnergySlotEntityPosition()
        {
            var x = GetBorderX(false) + ENERGY_SLOT_WIDTH * 0.5f;
            var y = GetScreenHeight() - ENERGY_SLOT_WIDTH * 0.5f;
            return new Vector2(x, y);
        }
        public static float GetScreenHeight()
        {
            return SCREEN_HEIGHT;
        }
        public static float GetBorderX(bool right)
        {
            return right ? RIGHT_BORDER : LEFT_BORDER;
        }
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

        public static float GetAttackBorderX(bool right)
        {
            return right ? ATTACK_RIGHT_BORDER : ATTACK_LEFT_BORDER;
        }
        public static float GetPickupBorderX(bool right)
        {
            return right ? PICKUP_RIGHT_BORDER : PICKUP_LEFT_BORDER;
        }
        public static float GetEnemyRightBorderX()
        {
            return ENEMY_RIGHT_BORDER;
        }
        public const float ENERGY_SLOT_WIDTH = 48;
        public const float MIN_PREVIEW_X = 1080;
        public const float MAX_PREVIEW_X = 1300;
        public const float MIN_PREVIEW_Y = 50;
        public const float MAX_PREVIEW_Y = 450;
        public const float GRID_SIZE = 80;
        public const float LAWN_HEIGHT = 600;
        public const float LEVEL_WIDTH = 1400;
        public const float CART_START_X = 150;
        public const float SCREEN_WIDTH = 800;
        public const float SCREEN_HEIGHT = 600;
        public const float LEFT_BORDER = 220;
        public const float RIGHT_BORDER = LEFT_BORDER + SCREEN_WIDTH;
        public const float PICKUP_LEFT_BORDER = LEFT_BORDER + 50;
        public const float PICKUP_RIGHT_BORDER = RIGHT_BORDER - 50;
        public const float ATTACK_LEFT_BORDER = LEFT_BORDER;
        public const float ATTACK_RIGHT_BORDER = RIGHT_BORDER;
        public const float ENEMY_RIGHT_BORDER = RIGHT_BORDER + 60;
    }
}
