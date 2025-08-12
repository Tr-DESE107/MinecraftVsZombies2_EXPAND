using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using System.Linq;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.ShadowCellCore)]
    public class ShadowCellCore : ArtifactDefinition
    {
        public ShadowCellCore(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_ENEMY_FAINT, PreEnemyFaintCallback, priority: -100);
        }
        private void PreEnemyFaintCallback(EntityCallbackParams param, CallbackResult result)
        {
            var enemy = param.entity;
            if (!enemy.IsHostileEntity())
                return;
            var level = enemy.Level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact?.Definition != this)
                    continue;

                //enemy.Revive();
                //enemy.Health = enemy.GetMaxHealth();
                //enemy.Charm(level.Option.LeftFaction);
                //result.SetFinalValue(false);

                bool isHead = RandomSkeleton.Contains(enemy.GetDefinitionID());

                if (!isHead)
                {

                    var randomID = GetRandomSkeletonID(enemy.RNG);
                    var spawnParam = enemy.GetSpawnParams();
                    spawnParam.SetProperty(EngineEntityProps.FACTION, enemy.GetFaction());
                    enemy.Spawn(randomID, enemy.Position, spawnParam);



                    artifact.Highlight();
                    enemy.PlaySound(VanillaSoundID.revived);
                    return;
                }
            }
        }

        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            //¹ÖÎï³ö¹Ö
            VanillaEnemyID.SkeletonHead,
            VanillaEnemyID.ZombieHead,
            VanillaEnemyID.RedEyeZombieHead,
            VanillaEnemyID.HostHead,
            VanillaEnemyID.dullahanHead,
            VanillaEnemyID.RaiderSkull,
        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            100,
            20,
            1,
            5,
            50,
            10
        };

        
    }
}
