﻿using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2Logic.IZombie
{
    public interface IIZombieMap
    {
        void InsertEntity(int column, int lane, NamespaceID entity);
        bool CanInsert(int column, int lane, NamespaceID entity);
        bool CanInsert(Vector2Int position, NamespaceID entity)
        {
            return CanInsert(position.x, position.y, entity);
        }
        Vector2Int[] GetAllGridPositions();
        LevelEngine Level { get; }
        int Rounds { get; }
        int Columns { get; }
        int Lanes { get; }
    }
}
