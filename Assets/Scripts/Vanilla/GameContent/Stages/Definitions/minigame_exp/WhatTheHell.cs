// Assets/Scripts/Vanilla/GameContent/Stages/Definitions/minigame_exp/AllUnlockedEnemiesStage.cs  
#nullable enable

using System.Linq;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Localization;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Definitions;
using MVZ2Logic.Level;
using MVZ2Logic.Localization;
using MVZ2Logic.Stats;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Definitions;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    [AutoStageDefinition(VanillaStageNames.WhatTheHell)]
    public partial class WhatTheHell : StageDefinition
    {
        public WhatTheHell(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new FinalWaveClearBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
            AddBehaviour(new StarshardStageBehaviour(this));
        }

        public override void OnSetup(LevelEngine level)
        {
            base.OnSetup(level);

            var game = Global.Game;
            NamespaceID[] unlockedEnemies = Global.Saves.GetUnlockedEnemies();

            var validSpawnIDs = unlockedEnemies
                .Select(e => VanillaSpawnID.GetFromEntity(e))
                .Where(spawnID =>
                {
                    if (!NamespaceID.IsValid(spawnID))
                        return false;

                    var spawnDef = game.GetSpawnDefinition(spawnID);
                    if (spawnDef == null)
                        return false;

                    // 过滤掉不适合的怪物  
                    //var entityDef = game.GetEntityDefinition(spawnDef.GetSpawnEntity());
                    //if (entityDef == null)
                    //    return false;

                    //// 排除无害怪物、非活跃怪物等  
                    //if (entityDef.GetProperty<bool>("mvz2:harmless") ?? false)
                    //    return false;
                    //if (entityDef.GetProperty<bool>("mvz2:notActiveEnemy") ?? false)
                    //    return false;
                    //if (entityDef.GetProperty<bool>("mvz2:noReward") ?? false)
                    //    return false;

                    return true;
                })
                .Distinct()
                .ToArray();

            if (validSpawnIDs.Length > 0)
            {
                level.SetEnemyPool(validSpawnIDs);
            }
            else
            {
                // 如果没有已解锁的怪物，使用默认怪物  
                level.SetEnemyPool(new NamespaceID[]
                {
                    VanillaSpawnID.zombie,
                    VanillaSpawnID.leatherCappedZombie
                });
            }
        }
    }
}
