using PVZEngine.Base;

namespace MVZ2Logic.Entities
{
    public class EntityCounterDefinition : Definition
    {
        public EntityCounterDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override string GetDefinitionType() => LogicDefinitionTypes.ENTITY_COUNTER;
        public string CounterName { get; set; }
    }
}
