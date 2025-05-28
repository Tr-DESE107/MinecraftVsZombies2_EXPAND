﻿using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace PVZEngine.Placements
{
    public abstract class PlacementDefinition : Definition
    {
        public PlacementDefinition(string nsp, string name, SpawnCondition condition) : base(nsp, name)
        {
            spawnCondition = condition;
        }
        public void AddMethod(PlaceMethod method)
        {
            methods.Add(method);
        }
        public bool RemoveMethod(PlaceMethod method)
        {
            return methods.Remove(method);
        }
        public bool ValidateSpawn(LawnGrid grid, EntityDefinition entity)
        {
            var e = spawnCondition.GetSpawnError(this, grid, entity);
            return !NamespaceID.IsValid(e);
        }
        public NamespaceID GetSpawnError(LawnGrid grid, EntityDefinition entity)
        {
            return spawnCondition.GetSpawnError(this, grid, entity);
        }
        public NamespaceID GetPlaceError(LawnGrid grid, EntityDefinition entity)
        {
            NamespaceID error = null;
            foreach (var method in methods)
            {
                var e = method.GetPlaceError(this, grid, entity);
                if (!NamespaceID.IsValid(e))
                {
                    return null;
                }
                if (error == null)
                {
                    error = e;
                }
            }
            return error;
        }
        public Entity PlaceEntity(LawnGrid grid, EntityDefinition entity, PlaceParams param)
        {
            foreach (var method in methods)
            {
                var e = method.GetPlaceError(this, grid, entity);
                if (!NamespaceID.IsValid(e))
                {
                    return method.PlaceEntity(this, grid, entity, param);
                }
            }
            return null;
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.PLACEMENT;
        private SpawnCondition spawnCondition;
        private List<PlaceMethod> methods = new List<PlaceMethod>();
    }
}
