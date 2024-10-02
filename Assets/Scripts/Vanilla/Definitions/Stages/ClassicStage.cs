using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Extensions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public partial class ClassicStage : StageDefinition, IPreviewStage
    {
        public ClassicStage(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new WaveStageBehaviour(this));
            AddBehaviour(new GemStageBehaviour(this));
        }
        public void SetSpawnEntries(IEnemySpawnEntry[] entries)
        {
            spawnEntries = entries;
        }
        public override IEnumerable<IEnemySpawnEntry> GetEnemyPool()
        {
            return spawnEntries;
        }

        #region 预览敌人
        public void CreatePreviewEnemies(LevelEngine level, Rect region)
        {
            var pool = GetEnemyPool();
            var validEnemies = pool.Select(e => e.GetSpawnDefinition(level.ContentProvider)?.EntityID);
            level.CreatePreviewEnemies(validEnemies, region);
        }
        void IPreviewStage.RemovePreviewEnemies(LevelEngine level)
        {
            level.RemovePreviewEnemies();
        }
        #endregion

        private IEnemySpawnEntry[] spawnEntries;
    }
}
