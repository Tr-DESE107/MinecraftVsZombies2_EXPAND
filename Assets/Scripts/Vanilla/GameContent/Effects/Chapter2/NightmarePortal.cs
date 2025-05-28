using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmarePortal)]
    public class NightmarePortal : EffectBehaviour
    {
        #region ���з���
        public NightmarePortal(string nsp, string name) : base(nsp, name)
        {

        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetSpawnTime(entity, MAX_SPAWN_TIME);
            entity.SetSortingOrder(-10);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var spawnTime = GetSpawnTime(entity);
            if (spawnTime > 0)
            {
                spawnTime--;
                SetSpawnTime(entity, spawnTime);
                if (spawnTime == 0)
                {
                    var enemyID = GetEnemyID(entity);
                    if (NamespaceID.IsValid(enemyID))
                    {
                        entity.SpawnWithParams(enemyID, entity.Position);
                    }
                }
            }
        }
        public static int GetSpawnTime(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_SPAWN_TIME);
        public static void SetSpawnTime(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_SPAWN_TIME, value);
        public static NamespaceID GetEnemyID(Entity entity) => entity.GetBehaviourField<NamespaceID>(ID, PROP_ENEMY_ID);
        public static void SetEnemyID(Entity entity, NamespaceID value) => entity.SetBehaviourField(ID, PROP_ENEMY_ID, value);
        #endregion

        #region �����ֶ�
        public static readonly NamespaceID ID = VanillaEffectID.nightmarePortal;
        public const int MAX_SPAWN_TIME = 15;
        public static readonly VanillaEntityPropertyMeta<int> PROP_SPAWN_TIME = new VanillaEntityPropertyMeta<int>("SpawnTime");
        public static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_ENEMY_ID = new VanillaEntityPropertyMeta<NamespaceID>("EnemyID");
        #endregion
    }
}