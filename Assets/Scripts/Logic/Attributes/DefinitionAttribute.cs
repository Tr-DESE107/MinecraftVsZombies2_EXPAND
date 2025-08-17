using PVZEngine;

namespace MVZ2Logic
{
    public class SeedOptionDefinitionAttribute : DefinitionAttribute
    {
        public SeedOptionDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.SEED_OPTION)
        {
        }
    }
    public class NoteDefinitionAttribute : DefinitionAttribute
    {
        public NoteDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.NOTE)
        {
        }
    }
    public class ArtifactDefinitionAttribute : DefinitionAttribute
    {
        public ArtifactDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.ARTIFACT)
        {
        }
    }
    public class HeldItemDefinitionAttribute : DefinitionAttribute
    {
        public HeldItemDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.HELD_ITEM)
        {
        }
    }
    public class HeldItemBehaviourDefinitionAttribute : DefinitionAttribute
    {
        public HeldItemBehaviourDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.HELD_ITEM_BEHAVIOUR)
        {
        }
    }
    public class IZombieLayoutDefinitionAttribute : DefinitionAttribute
    {
        public IZombieLayoutDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.I_ZOMBIE_LAYOUT)
        {
        }
    }
    public class CommandDefinitionAttribute : DefinitionAttribute
    {
        public CommandDefinitionAttribute(string name) : base(name, LogicDefinitionTypes.COMMAND)
        {
        }
    }
}
