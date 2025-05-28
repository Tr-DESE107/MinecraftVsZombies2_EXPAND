﻿using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Obstacles
{
    [EntityBehaviourDefinition(VanillaObstacleNames.monsterSpawner)]
    public class MonsterSpawner : ObstacleBehaviour
    {
        public MonsterSpawner(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_HUGE_WAVE_EVENT, PostHugeWaveEventCallback);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            KillConflictContraptions(entity);
            var mainCollider = entity.GetCollider(EntityCollisionHelper.NAME_MAIN);
            if (mainCollider != null)
            {
                mainCollider.SetEnabled(false);
            }
            SetEntityToSpawn(entity, VanillaEnemyID.zombie);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.Remove();
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);

            var level = entity.Level;
            float spinSpeed = 1;
            if (level.HasBuff<DelayedSpawnerTriggerBuff>())
            {
                spinSpeed = 5;
            }
            else
            {
                spinSpeed = Mathf.Lerp(1, 5, (level.CurrentWave / (float)level.GetWavesPerFlag()) % 1);
            }
            entity.SetAnimationFloat("SpinSpeed", spinSpeed);
        }
        private void PostHugeWaveEventCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            level.AddBuff<DelayedSpawnerTriggerBuff>();
        }
        public static void Trigger(Entity spawner)
        {
            if (TrySpawnEntity(spawner, out var spawned))
            {
                spawner.Spawn(VanillaEffectID.emberParticles, spawned.GetCenter());
            }
        }
        public static bool TrySpawnEntity(Entity spawner, out Entity spawned)
        {
            return TrySpawnEntity(spawner, GetEntityToSpawn(spawner), out spawned);
        }
        public static bool TrySpawnEntity(Entity spawner, NamespaceID id, out Entity spawned)
        {
            var lane = spawner.GetLane();
            var column = spawner.GetColumn();
            var level = spawner.Level;
            for (int i = 0; i < spawnGrids.Length; i++)
            {
                var offset = spawnGrids[i];
                var l = lane + offset.y;
                var c = column + offset.x;
                var grid = level.GetGrid(c, l);
                if (TrySpawnEntityAt(spawner, id, grid, out var s))
                {
                    spawned = s;
                    return true;
                }
            }
            spawned = null;
            return false;
        }
        public static bool TrySpawnEntityAt(Entity spawner, NamespaceID id, LawnGrid grid, out Entity spawned)
        {
            spawned = null;
            if (grid == null || !grid.CanSpawnEntity(id))
                return false;
            var pos = grid.GetEntityPosition();
            spawned = spawner.SpawnWithParams(id, pos);
            return true;
        }
        public static NamespaceID GetEntityToSpawn(Entity entity)
        {
            return entity.GetProperty<NamespaceID>(PROP_ENTITY_TO_SPAWN);
        }
        public static void SetEntityToSpawn(Entity entity, NamespaceID id)
        {
            entity.SetProperty(PROP_ENTITY_TO_SPAWN, id);
            entity.SetModelProperty("EntityToSpawn", id);
        }
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_ENTITY_TO_SPAWN = new VanillaEntityPropertyMeta<NamespaceID>("EntityToSpawn");
        public static readonly Vector2Int[] spawnGrids = new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
        };
    }
}
