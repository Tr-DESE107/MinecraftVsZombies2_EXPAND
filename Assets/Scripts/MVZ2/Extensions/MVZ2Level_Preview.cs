using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Extensions
{
    public static partial class MVZ2Level
    {
        public static void CreatePreviewEnemies(this LevelEngine level, IEnumerable<NamespaceID> validEnemies, Rect region)
        {
            List<NamespaceID> enemyIDToCreate = new List<NamespaceID>();
            foreach (var id in validEnemies)
            {
                var spawnDefinition = level.ContentProvider.GetSpawnDefinition(id);
                int count = spawnDefinition.GetPreviewCount();

                for (int i = 0; i < count; i++)
                {
                    enemyIDToCreate.Add(id);
                }
            }

            List<Entity> createdEnemies = new List<Entity>();
            float radius = 80;
            while (enemyIDToCreate.Count > 0)
            {
                var creatingEnemyId = enemyIDToCreate.ToArray();
                foreach (var id in creatingEnemyId)
                {
                    var x = UnityEngine.Random.Range(region.xMin, region.xMax);
                    var z = UnityEngine.Random.Range(region.yMin, region.yMax);
                    var y = level.GetGroundY(x, z);
                    Vector3 pos = new Vector3(x, y, z);

                    if (radius > 0 && createdEnemies.Any(e => Vector3.Distance(e.Pos, pos) < radius))
                        continue;

                    Entity enm = level.Spawn(id, pos, null);
                    enm.SetPreviewEnemy(true);
                    createdEnemies.Add(enm);

                    enemyIDToCreate.Remove(id);
                }
                radius--;
            }
        }
        public static void RemovePreviewEnemies(this LevelEngine level)
        {
            foreach (var enemy in level.FindEntities(e => e.IsPreviewEnemy()))
            {
                enemy.Remove();
            }
        }
    }
}
