using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.gemEffect)]
    public class GemEffect : EffectBehaviour
    {
        #region ���з���
        public GemEffect(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = MIN_TIMEOUT;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var level = entity.Level;

            float alpha = 1;
            var timeout = entity.Timeout;
            var vanishTime = VANISH_TICKS;
            var moveTime = MOVE_TICKS;
            if (timeout > vanishTime + moveTime)
            {
                entity.Velocity *= 0.9f;
            }
            else if (timeout > vanishTime)
            {
                var targetPos = GetMoveTargetPosition(entity);
                entity.Velocity = (targetPos - entity.Position) * 0.2f;
                alpha = 1;
            }
            else
            {
                if (timeout == vanishTime)
                {
                    level.RemoveDelayedMoney(entity);
                }

                var vanishLerp = 1 - timeout / (float)vanishTime;
                entity.SetDisplayScale(Vector3.one * Mathf.Lerp(1, 0.5f, vanishLerp));
                alpha = Mathf.Lerp(1, 0, vanishLerp);
            }
            var color = entity.GetTint(true);
            color.a = alpha;
            entity.SetTint(color);
        }
        public static Entity[] SpawnGemEffects(LevelEngine level, int money, Vector3 position, Entity spawner, bool mute = false, int timeout = 20)
        {
            var moneyPairs = gemMoneyDict.OrderByDescending(p => p.Value);
            var currentMoney = money;
            var totalCountToSpawn = 0;
            var gemsToSpawn = new Dictionary<GemType, int>();
            foreach (var pair in moneyPairs)
            {
                var count = currentMoney / pair.Value;
                currentMoney = currentMoney % pair.Value;
                gemsToSpawn.Add(pair.Key, count);
                totalCountToSpawn += count;
            }
            var spawnedGems = new List<Entity>();
            var currentIndex = 0;
            var anglePerGem = 180 / (totalCountToSpawn + 1);
            var startAngle = -90 + anglePerGem;
            foreach (var pair in gemsToSpawn)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    var gem = SpawnGemEffect(level, pair.Key, position, spawner, mute, timeout);
                    spawnedGems.Add(gem);

                    var angle = startAngle + anglePerGem * currentIndex;
                    gem.Velocity = (Quaternion.Euler(0, 0, angle) * Vector3.up) * 10;
                    currentIndex++;
                }
            }
            // ��ʣ�µ�Ǯ����
            if (currentMoney > 0)
            {
                level.AddMoney(currentMoney);
            }
            return spawnedGems.ToArray();
        }
        public static Entity SpawnGemEffect(LevelEngine level, GemType type, Vector3 position, Entity spawner, bool mute = false, int timeout = 0)
        {
            var effect = level.Spawn(VanillaEffectID.gemEffect, position, spawner);
            level.ShowMoney();
            level.AddDelayedMoney(effect, gemMoneyDict[type]);
            effect.ChangeModel(gemModelDict[type]);
            effect.Timeout = timeout + MIN_TIMEOUT;
            if (!mute)
            {
                effect.PlaySound(gemSoundDict[type]);
            }
            return effect;
        }
        #endregion

        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetMoneyPanelEntityPosition();
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        public const int MOVE_TICKS = 20;
        public const int VANISH_TICKS = 10;
        public const float COLLECTED_Z = -100;
        public const int MIN_TIMEOUT = VANISH_TICKS + MOVE_TICKS;
        private static readonly Dictionary<GemType, int> gemMoneyDict = new Dictionary<GemType, int>()
        {
            { GemType.Emerald, 10 },
            { GemType.Ruby, 50 },
            { GemType.Diamond, 1000 },
        };
        private static readonly Dictionary<GemType, NamespaceID> gemSoundDict = new Dictionary<GemType, NamespaceID>()
        {
            { GemType.Emerald, VanillaSoundID.coin },
            { GemType.Ruby, VanillaSoundID.coin },
            { GemType.Diamond, VanillaSoundID.diamond },
        };
        private static readonly Dictionary<GemType, NamespaceID> gemModelDict = new Dictionary<GemType, NamespaceID>()
        {
            { GemType.Emerald, VanillaModelID.emerald },
            { GemType.Ruby, VanillaModelID.ruby },
            { GemType.Diamond, VanillaModelID.diamond },
        };
        public enum GemType
        {
            Emerald,
            Ruby,
            Diamond
        }
    }
}