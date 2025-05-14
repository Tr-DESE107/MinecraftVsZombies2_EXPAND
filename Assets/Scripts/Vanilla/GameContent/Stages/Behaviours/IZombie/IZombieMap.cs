using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class IZombieMap
    {
        public IZombieMap(LevelEngine level, int columns, int lanes)
        {
            Level = level;
            Columns = columns;
            Lanes = lanes;
        }
        public void Apply()
        {
            foreach (var pair in entries)
            {
                var entry = pair.Value;
                entry.Apply(Level);
            }
            for (int lane = 0; lane < Lanes; lane++)
            {
                var x = Level.GetColumnX(Columns);
                var z = Level.GetLaneZ(lane) + Level.GetGridHeight() * 0.5f;
                var y = Level.GetGroundY(x, z);
                var pos = new Vector3(x, y, z);
                Level.Spawn(VanillaEffectID.redline, pos, null);
            }
        }
        public void InsertEntity(int column, int lane, NamespaceID entity)
        {
            if (column >= Columns)
                return;
            var definition = Level.Content.GetEntityDefinition(entity);
            if (definition == null)
                return;
            var key = new Vector2Int(column, lane);
            var value = new IZombieMapEntry(column, lane, entity, definition.GetGridLayersToTake());
            entries.Add(key, value);
        }
        public bool CanInsert(int column, int lane, NamespaceID entity)
        {
            if (column >= Columns)
                return false;
            var definition = Level.Content.GetEntityDefinition(entity);
            if (definition == null)
                return false;
            var layers = definition.GetGridLayersToTake();
            var entry = GetEntry(column, lane);
            if (entry != null)
            {
                if (entry.takenLayers != null && entry.takenLayers.Intersect(layers).Count() > 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool CanInsert(Vector2Int position, NamespaceID entity)
        {
            return CanInsert(position.x, position.y, entity);
        }
        public Vector2Int[] GetAllGridPositions()
        {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int y = 0; y < Lanes; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
            return positions.ToArray();
        }
        public IZombieMapEntry GetEntry(int column, int lane)
        {
            return GetEntry(new Vector2Int(column, lane));
        }
        public IZombieMapEntry GetEntry(Vector2Int position)
        {
            if (entries.TryGetValue(position, out var entry))
            {
                return entry;
            }
            return null;
        }
        public LevelEngine Level { get; }
        public int Columns { get; }
        public int Lanes { get; }
        private Dictionary<Vector2Int, IZombieMapEntry> entries = new Dictionary<Vector2Int, IZombieMapEntry>();
    }
}
