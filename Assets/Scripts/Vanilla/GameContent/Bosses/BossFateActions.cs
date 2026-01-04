#nullable enable    
using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using Tools.Mathematics;
using UnityEngine;
using PVZEngine;
using MVZ2.Vanilla.Grids;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Bosses
{
    public static class BossFateActions
    {
        // 添加属性定义    
        private static readonly VanillaEntityPropertyMeta<RandomGenerator> PROP_EVENT_RNG =
            new VanillaEntityPropertyMeta<RandomGenerator>("EventRNG");

        // 命运选择相关常量  
        [TranslateMsg("梦魇对话框标题")]
        public const string CHOOSE_FATE_TITLE = "选择你的命运";
        [TranslateMsg("梦魇对话框文本")]
        public const string CHOOSE_FATE_DESCRIPTION = "选吧。";

        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_DISABLE = "失能";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_COME_TRUE = "成真";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_PANDORAS_BOX = "潘多拉的魔盒";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_INSANITY = "疯狂";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_DECREPIFY = "衰老";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_BIOHAZARD = "尸潮";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_THE_LURKER = "深潜者";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_BLACK_SUN = "黑太阳";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_HOST_ARRIVAL = "宿主降临";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_BIGBANG = "BIGBANG";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_AMPUTATION = "截肢";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_BONE_PILE = "骨堆";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_REBIRTH = "新生";
        [TranslateMsg("梦魇选项")]
        public const string FATE_TEXT_SHADOW_CHASING = "逐影";

        // 命运选项数组  
        private static readonly int[] fateOptions = new int[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13
        };

        private static readonly string[] fateTexts = new string[]
        {
            FATE_TEXT_DISABLE, FATE_TEXT_COME_TRUE, FATE_TEXT_PANDORAS_BOX,
            FATE_TEXT_INSANITY, FATE_TEXT_DECREPIFY, FATE_TEXT_BIOHAZARD,
            FATE_TEXT_THE_LURKER, FATE_TEXT_BLACK_SUN, FATE_TEXT_HOST_ARRIVAL,
            FATE_TEXT_BIGBANG, FATE_TEXT_AMPUTATION, FATE_TEXT_BONE_PILE, FATE_TEXT_REBIRTH,
            FATE_TEXT_SHADOW_CHASING
        };

        /// <summary>  
        /// 显示命运选择对话框  
        /// </summary>  
        public static void ShowFateChoice(Entity boss, int optionCount = 3)
        {
            var level = boss.Level;
            level.PauseGame(100);

            var title = Global.Localization.GetText(CHOOSE_FATE_TITLE);
            var desc = Global.Localization.GetText(CHOOSE_FATE_DESCRIPTION);

            var rng = GetEventRNG(boss);
            var selected = rng != null ?
                fateOptions.RandomTake(optionCount, rng).ToArray() :
                fateOptions.Take(optionCount).ToArray();

            var options = selected.Select(i => GetFateOptionText(boss.RNG, i)).ToArray();

            level.ShowDialog(title, desc, options, (i) =>
            {
                var option = selected[i];
                ExecuteFate(boss, option);
                level.ResumeGameDelayed(100);
            });
        }

        /// <summary>  
        /// 执行指定的命运  
        /// </summary>  
        public static void ExecuteFate(Entity boss, int option)
        {
            switch (option)
            {
                case 0: Disable(boss); break;
                case 1: ComeTrue(boss); break;
                case 2: PandorasBox(boss); break;
                case 3: Insanity(boss); break;
                case 4: Decrepify(boss); break;
                case 5: Biohazard(boss); break;
                case 6: TheLurker(boss); break;
                case 7: BlackSun(boss); break;
                case 8: HostArrival(boss); break;
                case 9: BigBang(boss); break;
                case 10: Amputation(boss); break;
                case 11: BonePile(boss); break;
                case 12: Rebirth(boss); break;
                case 13: ShadowChasing(boss); break;
            }
        }

        /// <summary>  
        /// 获取命运选项文本  
        /// </summary>  
        private static string GetFateOptionText(RandomGenerator rng, int option)
        {
            var index = Array.IndexOf(fateOptions, option);
            int randomInt = rng.Next(0, 13);
            string text = randomInt < 12 ? fateTexts[index] : "???";
            return Global.Localization.GetText(text);
        }

        public static void Disable(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.powerOff);
            var level = boss.Level;
            var contraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsHostile(boss) && e.CanDeactive());
            foreach (var contraption in contraptions)
            {
                contraption.ShortCircuit(300, new EntitySourceReference(boss));
            }
        }

        public static void ComeTrue(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.nyaightmareScream);
            var level = boss.Level;
            var targets = level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsFriendly(boss) && !e.IsEntityOf(VanillaEnemyID.ghast));
            foreach (var enemy in targets)
            {
                boss.SpawnWithParams(VanillaEnemyID.ghast, enemy.Position)?.Let(e =>
                {
                    e.AddBuff<NightmareComeTrueBuff>();
                });
                enemy.Remove();
            }
        }

        public static void PandorasBox(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.odd);
            var level = boss.Level;
            var eventRng = GetEventRNG(boss);
            if (eventRng == null)
                return;
            var rng = new RandomGenerator(eventRng.Next());
            var contraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsHostile(boss));
            foreach (var contraption in contraptions)
            {
                contraption.ClearTakenGrids();
            }
            var grids = level.GetAllGrids();
            foreach (var contraption in contraptions)
            {
                var placementID = contraption.Definition.GetPlacementID();
                if (placementID == null)
                    continue;
                var placementDef = level.Content.GetPlacementDefinition(placementID);
                if (placementDef == null)
                    continue;
                var targetGrids = grids.Where(g => g.CanSpawnEntity(contraption.GetDefinitionID()));
                if (targetGrids.Count() <= 0)
                    continue;
                var grid = targetGrids.Random(rng);
                contraption.Position = grid.GetEntityPosition();
                contraption.UpdateTakenGrids();
            }
        }

        public static void Insanity(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.confuse);
            var level = boss.Level;
            var rng = GetEventRNG(boss);
            var contraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsHostile(boss) && !e.IsLoyal());
            var charmTargets = rng != null ? contraptions.RandomTake(Mathf.Max(1, contraptions.Length / 3), rng) : contraptions.Take(Mathf.Max(1, contraptions.Length / 3));
            foreach (var target in charmTargets)
            {
                target.CharmPermanent(boss.GetFaction(), new EntitySourceReference(boss));
            }
            var enemies = level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsFriendly(boss));
            foreach (var enemy in enemies)
            {
                var attackBuff = enemy.AddBuff<AttackSpeedBuff>();
                attackBuff?.SetProperty(AttackSpeedBuff.PROP_ATTACK_SPEED_MULTIPLIER, 2f);
            }
        }

        public static void Decrepify(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.decrepify);
            boss.Level.AddBuff<NightmareDecrepifyBuff>();
        }

        public static void Biohazard(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.biohazard);
            boss.PlaySound(VanillaSoundID.nightmarePortal);
            var level = boss.Level;
            var enemyPool = new NamespaceID[]
            {
                VanillaEnemyID.HostZombie,
                VanillaEnemyID.ironHelmettedZombie,
                VanillaEnemyID.gargoyle
            };
            var rng = GetEventRNG(boss);
            for (int column = 0; column < 2; column++)
            {
                float x = level.GetEntityColumnX(level.GetMaxColumnCount() - 1 - column);
                for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
                {
                    var z = level.GetEntityLaneZ(lane);
                    var y = level.GetGroundY(x, z);
                    Vector3 pos = new Vector3(x, y, z);
                    var randomEnemy = enemyPool.Random(rng);
                    SpawnPortal(boss, pos, randomEnemy);
                }
            }
        }

        public static void TheLurker(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.splashBig);
            boss.PlaySound(VanillaSoundID.lurker);
            var rng = GetEventRNG(boss);
            var level = boss.Level;
            level.ShakeScreen(50, 0, 30);  // 现在这个方法可以使用了    
            var waterContraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsHostile(boss) && e.IsOnWater());
            var destroyCount = Mathf.CeilToInt(waterContraptions.Length * 0.5f);
            var destroyTargets = rng != null ? waterContraptions.RandomTake(destroyCount, rng) : waterContraptions.Take(destroyCount);
            foreach (var target in destroyTargets)
            {
                target.Die(boss);
            }
            int spawnCount = rng != null ? rng.Next(3, 8) : UnityEngine.Random.Range(3, 8);
            var boatZombies = new NamespaceID[]
            {
                VanillaEnemyID.zombie,
                VanillaEnemyID.leatherCappedZombie,
                VanillaEnemyID.ironHelmettedZombie
            };
            for (int i = 0; i < spawnCount; i++)
            {
                int lane = rng != null ? rng.Next(0, level.GetMaxLaneCount()) : UnityEngine.Random.Range(0, level.GetMaxLaneCount());
                var grid = level.GetGrid(lane);
                if (grid == null || !grid.IsWater()) continue;
                float x = level.GetEntityColumnX(level.GetMaxColumnCount() - 1);
                float z = level.GetEntityLaneZ(lane);
                float y = level.GetGroundY(x, z);
                Vector3 pos = new Vector3(x, y, z);
                var boatZombieID = boatZombies[rng != null ? rng.Next(0, boatZombies.Length) : UnityEngine.Random.Range(0, boatZombies.Length)];
                boss.Spawn(boatZombieID, pos)?.Let(e =>
                {
                    e.AddBuff<BoatBuff>();
                });
            }
        }

        public static void BlackSun(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.reverseVampire);
            boss.PlaySound(VanillaSoundID.confuse);
            var level = boss.Level;
            level.AddBuff<SlendermanBlackSunBuff>();
        }

        public static void HostArrival(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.scream);
            boss.PlaySound(VanillaSoundID.biohazard);

            var level = boss.Level;
            var rng = GetEventRNG(boss);
            int spawnCount = rng != null ? rng.Next(4, 8) : UnityEngine.Random.Range(4, 8);

            List<Vector2Int> validPositions = new List<Vector2Int>();
            int maxColumnCount = level.GetMaxColumnCount();
            int maxLaneCount = level.GetMaxLaneCount();

            for (int column = 1; column < maxColumnCount; column++)
            {
                for (int lane = 0; lane < maxLaneCount; lane++)
                {
                    var grid = level.GetGrid(lane);
                    if (grid != null && !grid.IsWater())
                    {
                        validPositions.Add(new Vector2Int(column, lane));
                    }
                }
            }

            for (int i = 0; i < spawnCount; i++)
            {
                if (validPositions.Count <= 4) break;

                int index = rng != null ? rng.Next(0, validPositions.Count) : UnityEngine.Random.Range(0, validPositions.Count);
                var posInfo = validPositions[index];
                validPositions.RemoveAt(index);

                float x = level.GetEntityColumnX(posInfo.x);
                float z = level.GetEntityLaneZ(posInfo.y);
                float y = level.GetGroundY(x, z) + 300f;
                Vector3 spawnPos = new Vector3(x, y, z);

                boss.Spawn(VanillaEnemyID.BloodlustHostZombie, spawnPos)?.Let(e =>
                {
                    e.AddBuff<TerrorParasitizedBuff>();
                });
            }
        }

        public static void BigBang(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.smash);
            var level = boss.Level;
            var rng = GetEventRNG(boss);
            var contraptions = level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsHostile(boss)).ToArray();
            int explosionCount = Mathf.Min(rng != null ? rng.Next(2, 5) : UnityEngine.Random.Range(2, 5), contraptions.Length);
            for (int i = 0; i < explosionCount; i++)
            {
                if (contraptions.Length == 0) break;
                var targetIndex = rng != null ? rng.Next(0, contraptions.Length) : UnityEngine.Random.Range(0, contraptions.Length);
                var target = contraptions[targetIndex];
                var remainingContraptions = contraptions.ToList();
                remainingContraptions.RemoveAt(targetIndex);
                contraptions = remainingContraptions.ToArray();
                float range = 80f;
                var damageEffects = new DamageEffectList(
                    VanillaDamageEffects.MUTE,
                    VanillaDamageEffects.IGNORE_ARMOR,
                    VanillaDamageEffects.EXPLOSION
                );
                var damageOutputs = boss.Explode(
                    target.Position,
                    range,
                    boss.GetFaction(),
                    200,
                    damageEffects
                );
                foreach (var output in damageOutputs)
                {
                    if (output == null) continue;
                    var result = output.BodyResult;
                    if (result != null && result.Fatal)
                    {
                        var targetEntity = output.Entity;
                        var distance = (targetEntity.Position - target.Position).magnitude;
                        var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                        targetEntity.Velocity = targetEntity.Velocity + Vector3.up * speed;
                    }
                }
                Explosion.Spawn(boss, target.GetCenter(), range);
                boss.Level.Spawn(VanillaEffectID.mineDebris, target.Position, boss);
                boss.PlaySound(VanillaSoundID.explosion);
                boss.Level.ShakeScreen(10, 0, 15);  // 现在这个方法可以使用了    
            }
        }

        public static void Amputation(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.boneWallBuild);
            boss.PlaySound(VanillaSoundID.nightmarePortal);
            var level = boss.Level;
            var rng = GetEventRNG(boss);
            int[] columns = { 8, 7, 6 };
            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
            {
                for (int i = 0; i < 3; i++)
                {
                    float x = level.GetEntityColumnX(columns[i]);
                    float z = level.GetEntityLaneZ(lane);
                    float y = level.GetGroundY(x, z);
                    Vector3 pos = new Vector3(x, y, z);
                    SpawnPortal(boss, pos, VanillaEnemyID.caveSpider);
                }
            }
        }

        public static void BonePile(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.boneWallBuild);
            boss.PlaySound(VanillaSoundID.nightmarePortal);
            var level = boss.Level;
            var rng = GetEventRNG(boss);
            int[] columns = { 8, 7, 6 };
            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
            {
                for (int i = 0; i < 3; i++)
                {
                    float x = level.GetEntityColumnX(columns[i]);
                    float z = level.GetEntityLaneZ(lane);
                    float y = level.GetGroundY(x, z);
                    Vector3 pos = new Vector3(x, y, z);
                    SpawnPortal(boss, pos, VanillaEnemyID.boneWall);
                }
            }
        }

        public static void Rebirth(Entity boss)
        {
            boss.PlaySound(VanillaSoundID.revived);
            var level = boss.Level;
            var enemies = level.FindEntities(e => e.Type == EntityTypes.ENEMY && e.IsFriendly(boss));
            foreach (var enemy in enemies)
            {
                var regenBuff = enemy.AddBuff<RegenerationBuff>();
                regenBuff?.SetProperty(RegenerationBuff.PROP_HEAL_AMOUNT, 1f);
                regenBuff?.SetProperty(RegenerationBuff.PROP_TIMEOUT, 600);
            }
        }

        public static void ShadowChasing(Entity boss)
        {
            //逐影：在第8列随机召唤x个梦魇弟子  
            boss.PlaySound(VanillaSoundID.nightmarePortal);
            var level = boss.Level;
            var rng = GetEventRNG(boss);

            // 设置召唤数量，可以随机或固定  
            int spawnCount = rng != null ? rng.Next(1, 3) : UnityEngine.Random.Range(1, 3);

            // 收集第8列的所有有效位置  
            List<Vector2Int> validPositions = new List<Vector2Int>();
            int column = 8; // 第8列  

            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
            {
                validPositions.Add(new Vector2Int(column, lane));
            }

            // 随机选择位置进行召唤  
            for (int i = 0; i < spawnCount && validPositions.Count > 0; i++)
            {
                int index = rng != null ? rng.Next(0, validPositions.Count) : UnityEngine.Random.Range(0, validPositions.Count);
                var posInfo = validPositions[index];
                validPositions.RemoveAt(index);

                float x = level.GetEntityColumnX(posInfo.x);
                float z = level.GetEntityLaneZ(posInfo.y);
                float y = level.GetGroundY(x, z);
                Vector3 spawnPos = new Vector3(x, y, z);

                SpawnPortal(boss, spawnPos, VanillaEnemyID.NightmareDisciple);
            }
        }

        // 辅助方法也需要是静态的    
        public static Entity? SpawnPortal(Entity boss, Vector3 position, NamespaceID enemyID)
        {
            return boss.SpawnWithParams(VanillaEffectID.nightmarePortal, position)?.Let(e =>
            {
                NightmarePortal.SetEnemyID(e, enemyID);
            });
        }

        public static Entity? SpawnEntityWithBoat(Entity boss, Vector3 pos, int lane, NamespaceID enemyID)
        {
            var grid = boss.Level.GetGrid(lane);
            if (grid != null && grid.IsWater())
            {
                return boss.Spawn(enemyID, pos)?.Let(e =>
                {
                    e.AddBuff<BoatBuff>();
                });
            }
            return boss.Spawn(enemyID, pos);
        }

        public static void SetEventRNG(Entity boss, RandomGenerator value)
        {
            boss.SetBehaviourField(boss.Definition.GetID(), PROP_EVENT_RNG, value);
        }

        // 修复后的 GetEventRNG 方法  
        public static RandomGenerator? GetEventRNG(Entity boss)
        {
            // 尝试获取boss特定的EventRNG属性  
            var eventRNG = boss.GetBehaviourField<RandomGenerator>(boss.Definition.GetID(), PROP_EVENT_RNG);

            // 如果没有特定的EventRNG，使用boss的基础RNG  
            return eventRNG ?? boss.RNG;
        }
    }
}