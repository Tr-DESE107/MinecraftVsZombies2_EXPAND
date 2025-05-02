using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.randomChina)]
    public class RandomChina : ContraptionBehaviour
    {
        public RandomChina(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity contraption)
        {
            base.UpdateLogic(contraption);
            var state = contraption.GetHealthState(3);
            contraption.SetAnimationInt("HealthState", state);
        }

        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            var grid = entity.GetGrid();
            if (grid == null)
                return;

            var game = Global.Game;
            var level = entity.Level;
            var rng = entity.RNG;
            entity.ClearTakenGrids();
            var unlockedContraptions = game.GetUnlockedContraptions();
            var validContraptions = unlockedContraptions.Where(id =>
            {
                if (!game.IsContraptionInAlmanac(id))
                    return false;
                var def = game.GetEntityDefinition(id);
                if (def.IsUpgradeBlueprint())
                    return false;
                return grid.CanPlaceEntity(id);
            });
            if (validContraptions.Count() <= 0)
                return;
            var contraptionID = validContraptions.Random(rng);
            var spawnParam = entity.GetSpawnParams();
            var spawned = entity.Spawn(contraptionID, entity.Position, spawnParam);
            if (spawned != null && spawned.HasBuff<NocturnalBuff>())
            {
                spawned.RemoveBuffs<NocturnalBuff>();
            }
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
            var rng = new RandomGenerator(contraption.RNG.Next());
            var id = events.Random(rng);
            RunEvent(contraption, id, rng);
            var nameKey = eventNames[id];
            contraption.Level.ShowAdvice(VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME, nameKey, 0, 90);
        }
        private void RunEvent(Entity contraption, int id, RandomGenerator rng)
        {
            switch (id)
            {
                case EVENT_OBSIDIAN_PRISON:
                    RunEventObsidianPrisons(contraption);
                    break;
                case EVENT_CHINA_TOWN:
                    RunEventChinaTown(contraption);
                    break;
                case EVENT_THE_TOWER:
                    RunEventTheTower(contraption, rng);
                    break;
                case EVENT_REDSTONE_READY:
                    RunEventRedstoneReady(contraption, rng);
                    break;
                case EVENT_ACE_OF_DIAMONDS:
                    RunEventAceOfDiamonds(contraption, rng);
                    break;
                case EVENT_HELL_METAL:
                    RunEventHellMetal(contraption);
                    break;
            }
        }
        private void RunEventObsidianPrisons(Entity contraption)
        {
            var level = contraption.Level;
            foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsHostile(contraption) && e.ExistsAndAlive()))
            {
                for (int i = 0; i < 5; i++)
                {
                    int column = enemy.GetColumn();
                    var lane = enemy.GetLane();
                    switch (i)
                    {
                        case 1:
                            lane--;
                            break;
                        case 2:
                            column++;
                            break;
                        case 3:
                            lane++;
                            break;
                        case 4:
                            column--;
                            break;
                    }

                    if (column >= 0 && column < level.GetMaxColumnCount() && lane >= 0 && lane < level.GetMaxLaneCount())
                    {
                        var grid = level.GetGrid(column, lane);
                        if (grid.CanPlaceEntity(VanillaContraptionID.obsidian))
                        {
                            var spawnParams = contraption.GetSpawnParams();
                            var pos = grid.GetEntityPosition();
                            contraption.Spawn(VanillaContraptionID.obsidian, pos, spawnParams);
                        }
                    }
                }
            }
        }
        private void RunEventChinaTown(Entity contraption)
        {
            var level = contraption.Level;
            for (int column = 0; column < level.GetMaxColumnCount(); column++)
            {
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var grid = level.GetGrid(column, lane);
                    if (grid.CanPlaceEntity(VanillaContraptionID.randomChina))
                    {
                        var spawnParams = contraption.GetSpawnParams();
                        var pos = grid.GetEntityPosition();
                        contraption.Spawn(VanillaContraptionID.randomChina, pos, spawnParams);
                    }
                }
            }
        }
        private void RunEventTheTower(Entity contraption, RandomGenerator rng)
        {
            const int tntCount = 16;
            var level = contraption.Level;
            for (int i = 0; i < tntCount; i++)
            {
                float x = rng.Next(VanillaLevelExt.ATTACK_LEFT_BORDER, VanillaLevelExt.ATTACK_RIGHT_BORDER);
                float y = rng.Next(600f, 2000f);
                float z = rng.Next(level.GetGridBottomZ(), level.GetGridTopZ());
                var spawnParams = contraption.GetSpawnParams();
                contraption.Spawn(VanillaProjectileID.flyingTNT, new Vector3(x, y, z), spawnParams);
            }
        }
        private void RunEventRedstoneReady(Entity contraption, RandomGenerator rng)
        {
            const int redstoneCount = 20;
            var level = contraption.Level;
            for (int i = 0; i < redstoneCount; i++)
            {
                float radius = rng.Next(0, 0.2f);
                float angle = rng.Next(0, 360 * Mathf.Deg2Rad);
                float horizontal = Mathf.Cos(angle);
                float vertical = Mathf.Sin(angle);
                float x = horizontal * radius;
                float z = vertical * radius;

                float y = level.GetGroundY(x, z);
                var redstone = contraption.Spawn(VanillaPickupID.redstone, new Vector3(contraption.Position.x + x, y + 10, contraption.Position.z + z));
                redstone.Velocity = new Vector3(x * 20f, 4f, z * 20f);
            }
        }
        private void RunEventAceOfDiamonds(Entity contraption, RandomGenerator rng)
        {
            var level = contraption.Level;
            foreach (var pickup in level.GetEntities(EntityTypes.PICKUP))
            {
                if (pickup.IsCollected() || pickup.IsImportantPickup())
                    continue;
                var emerald = contraption.Spawn(VanillaPickupID.emerald, pickup.Position);
                emerald.Velocity = Vector3.up * 2f;
                pickup.Remove();
            }
            foreach (var enemy in level.GetEntities(EntityTypes.ENEMY))
            {
                var emerald = contraption.Spawn(VanillaPickupID.emerald, enemy.Position);
                emerald.Velocity = Vector3.up * 2f;
                enemy.Remove();
            }
        }
        private void RunEventHellMetal(Entity contraption)
        {
            var level = contraption.Level;
            contraption.PlaySound(VanillaSoundID.armorUp);
            foreach (var enemy in level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.ExistsAndAlive()))
            {
                enemy.EquipMainArmor(VanillaArmorID.ironHelmet);
            }
        }

        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_OBSIDIAN_PRISON = "黑曜石囚牢";
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_CHINA_TOWN = "陶瓷镇";
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_THE_TOWER = "塔-XVI";
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_REDSTONE_READY = "红石俱备";
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_ACE_OF_DIAMONDS = "方片A";
        [TranslateMsg("随机瓷器事件名称", VanillaStrings.CONTEXT_RANDOM_CHINA_EVENT_NAME)]
        public const string EVENT_NAME_HELL_METAL = "地狱金属";

        public const int EVENT_OBSIDIAN_PRISON = 0;
        public const int EVENT_CHINA_TOWN = 1;
        public const int EVENT_THE_TOWER = 2;
        public const int EVENT_REDSTONE_READY = 3;
        public const int EVENT_ACE_OF_DIAMONDS = 4;
        public const int EVENT_HELL_METAL = 5;
        public static readonly int[] events = new int[]
        {
            EVENT_OBSIDIAN_PRISON,
            EVENT_CHINA_TOWN,
            EVENT_THE_TOWER,
            EVENT_REDSTONE_READY,
            EVENT_ACE_OF_DIAMONDS,
            EVENT_HELL_METAL,
        };
        public static readonly string[] eventNames = new string[]
        {
            EVENT_NAME_OBSIDIAN_PRISON,
            EVENT_NAME_CHINA_TOWN,
            EVENT_NAME_THE_TOWER,
            EVENT_NAME_REDSTONE_READY,
            EVENT_NAME_ACE_OF_DIAMONDS,
            EVENT_NAME_HELL_METAL,
        };
    }
}
