using System;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace PVZEngine.Level
{
    public class AreaDefinitionAttribute : DefinitionAttribute
    {
        public AreaDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.AREA)
        {
        }
    }
    public class RechargeDefinitionAttribute : DefinitionAttribute
    {
        public RechargeDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.RECHARGE)
        {
        }
    }
    public class SeedDefinitionAttribute : DefinitionAttribute
    {
        public SeedDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.SEED)
        {
        }
    }
    public class SpawnDefinitionAttribute : DefinitionAttribute
    {
        public SpawnDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.SPAWN)
        {
        }
    }
    public class StageDefinitionAttribute : DefinitionAttribute
    {
        public StageDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.STAGE)
        {
        }
    }
    public class BuffDefinitionAttribute : DefinitionAttribute
    {
        public BuffDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.BUFF)
        {
        }
    }
    public class GridDefinitionAttribute : DefinitionAttribute
    {
        public GridDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.GRID)
        {
        }
    }
    public class EntityDefinitionAttribute : DefinitionAttribute
    {
        public EntityDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.ENTITY)
        {
        }
    }
    public class ArmorDefinitionAttribute : DefinitionAttribute
    {
        public ArmorDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.ARMOR)
        {
        }
    }
    public class EntityBehaviourDefinitionAttribute : DefinitionAttribute
    {
        public EntityBehaviourDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.ENTITY_BEHAVIOUR)
        {
        }
    }
    public class PlacementDefinitionAttribute : DefinitionAttribute
    {
        public PlacementDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.PLACEMENT)
        {
        }
    }
    public class ShellDefinitionAttribute : DefinitionAttribute
    {
        public ShellDefinitionAttribute(string name) : base(name, EngineDefinitionTypes.SHELL)
        {
        }
    }
}
