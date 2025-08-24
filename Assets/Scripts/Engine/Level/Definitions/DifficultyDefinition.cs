using PVZEngine.Base;

namespace PVZEngine.Definitions
{
    public class DifficultyDefinition : Definition
    {
        public DifficultyDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.DIFFICULTY;
    }
}
