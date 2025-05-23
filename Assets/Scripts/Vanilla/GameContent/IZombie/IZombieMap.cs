using System.Collections.Generic;
using System.Linq;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.UIElements;

namespace MVZ2Logic.IZombie
{
    public class IZombieMap : IIZombieMap
    {
        public IZombieMap(LevelEngine level, int columns, int lanes, int rounds)
        {
            Level = level;
            Columns = columns;
            Lanes = lanes;
            Rounds = rounds;
        }
        public void Apply()
        {
            foreach (var entry in entries)
            {
                entry.Apply(Level);
            }
        }
        public void InsertEntity(int column, int lane, NamespaceID entity)
        {
            if (column < 0 || column >= Columns)
                return;
            if (lane < 0 || lane >= Lanes)
                return;
            var definition = Level.Content.GetEntityDefinition(entity);
            if (definition == null)
                return;
            var value = new IZombieMapEntry(column, lane, entity, definition.GetGridLayersToTake());
            entries.Add(value);
        }
        public bool CanInsert(int column, int lane, NamespaceID entity)
        {
            if (column >= Columns)
                return false;
            var definition = Level.Content.GetEntityDefinition(entity);
            if (definition == null)
                return false;
            var layers = definition.GetGridLayersToTake();
            var entries = GetEntriesAt(column, lane);
            foreach (var entry in entries)
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
        public IZombieMapEntry[] GetEntriesAt(int column, int lane)
        {
            return entries.Where(e => e.column == column && e.lane == lane).ToArray();
        }
        public IZombieMapEntry[] GetEntriesAt(Vector2Int position)
        {
            return GetEntriesAt(position.x, position.y);
        }
        public IZombieMapEntry GetEntry(int column, int lane)
        {
            return entries.Find(e => e.column == column && e.lane == lane);
        }
        public IZombieMapEntry GetEntry(Vector2Int position)
        {
            return GetEntry(position.x, position.y);
        }
        public LevelEngine Level { get; }
        public int Columns { get; }
        public int Lanes { get; }
        public int Rounds { get; }
        private List<IZombieMapEntry> entries = new List<IZombieMapEntry>();
    }
}
