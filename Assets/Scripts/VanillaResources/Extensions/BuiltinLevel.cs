using System.Linq;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class BuiltinLevel
    {
        public static Rect GetEnemySpawnRect()
        {
            return new Rect(MIN_PREVIEW_X, MIN_PREVIEW_Y, MAX_PREVIEW_X - MIN_PREVIEW_X, MAX_PREVIEW_Y - MIN_PREVIEW_Y);
        }
        public static float GetScreenHeight()
        {
            return SCREEN_HEIGHT;
        }
        public static float GetBorderX(bool right)
        {
            return right ? RIGHT_BORDER : LEFT_BORDER;
        }
        public static void SpawnCarts(this LevelEngine game, NamespaceID cartRef, float x, float xInterval)
        {
            var carts = game.GetEntities(EntityTypes.CART);
            for (int i = 0; i < game.GetMaxLaneCount(); i++)
            {
                if (carts.Any(c => c.GetLane() == i && c.State == EntityStates.IDLE))
                    continue;
                Entity cart = game.Spawn(cartRef, new Vector3(x - i * xInterval, 0, game.GetEntityLaneZ(i)), null);
            }
        }
        public static bool IsAutoCollect(this LevelEngine game)
        {
            return game.GetProperty<bool>(BuiltinStageProps.AUTO_COLLECT);
        }
        public static bool IsNoProduction(this LevelEngine game)
        {
            return game.GetProperty<bool>(BuiltinStageProps.NO_PRODUCTION);
        }
        public static void SetNoProduction(this LevelEngine game, bool value)
        {
            game.SetProperty(BuiltinStageProps.NO_PRODUCTION, value);
        }
        public static NamespaceID GetStartTalk(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(BuiltinStageProps.START_TALK);
        }
        public static NamespaceID GetEndTalk(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(BuiltinStageProps.END_TALK);
        }
        public static NamespaceID GetMapTalk(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(BuiltinStageProps.MAP_TALK);
        }
        public static NamespaceID GetEndNoteID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(BuiltinStageProps.END_NOTE_ID);
        }
        public static int GetStarshardCount(this LevelEngine game)
        {
            return game.GetProperty<int>(BuiltinLevelProps.STARSHARD_COUNT);
        }
        public static void SetStarshardCount(this LevelEngine game, int value)
        {
            game.SetProperty(BuiltinLevelProps.STARSHARD_COUNT, value);
        }
        public static void AddStarshardCount(this LevelEngine game, int value)
        {
            game.SetStarshardCount(GetStarshardCount(game) + value);
        }
        public static bool IsPickaxeDisabled(this LevelEngine level)
        {
            return level.GetProperty<bool>(BuiltinLevelProps.PICKAXE_DISABLED);
        }
        public static string GetPickaxeDisableMessage(this LevelEngine level)
        {
            return level.GetProperty<string>(BuiltinLevelProps.PICKAXE_DISABLE_MESSAGE);
        }
        public static float GetDoorZ(this LevelEngine game)
        {
            return game.GetProperty<float>(BuiltinAreaProps.DOOR_Z);
        }
        public static Vector3 GetLastEnemyPosition(this LevelEngine game)
        {
            return game.GetProperty<Vector3>(BuiltinLevelProps.LAST_ENEMY_POSITION);
        }
        public static NamespaceID GetMusicID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(BuiltinLevelProps.MUSIC_ID);
        }
        public static void SetLastEnemyPosition(this LevelEngine game, Vector3 value)
        {
            game.SetProperty(BuiltinLevelProps.LAST_ENEMY_POSITION, value);
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
        public static string GetLevelName(this LevelEngine level)
        {
            return level.GetProperty<string>(BuiltinStageProps.LEVEL_NAME);
        }
        public static void SetLevelName(this StageDefinition stage, string name)
        {
            stage.SetProperty(BuiltinStageProps.LEVEL_NAME, name);
        }
        public static int GetDayNumber(this LevelEngine level)
        {
            return level.GetProperty<int>(BuiltinStageProps.DAY_NUMBER);
        }
        public static void SetDayNumber(this StageDefinition stage, int number)
        {
            stage.SetProperty(BuiltinStageProps.DAY_NUMBER, number);
        }
        public const float ENERGY_SLOT_WIDTH = 48;

        public const float MONEY_PANEL_X_TO_LEFT = 16;
        public const float MONEY_PANEL_Y_TO_BOTTOM = 32;

        public const float STARSHARD_X_TO_LEFT = 128 + 165 + 16;
        public const float STARSHARD_Y_TO_BOTTOM = 32;

        public const float MIN_PREVIEW_X = 1080;
        public const float MAX_PREVIEW_X = 1300;
        public const float MIN_PREVIEW_Y = 50;
        public const float MAX_PREVIEW_Y = 450;
        public const float GRID_SIZE = 80;
        public const float LAWN_HEIGHT = 600;
        public const float LEVEL_WIDTH = 1400;
        public const float CART_START_X = 150;
        public const float CART_TARGET_X = LEFT_BORDER;
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
