using PVZEngine.Entities;

namespace MVZ2Logic.Entities
{
    public class MetaEntityDefinition : EntityDefinition
    {
        public MetaEntityDefinition(int type, string nsp, string name) : base(nsp, name)
        {
            this.type = type;
        }
        public override int Type => type;
        private int type;
        private EntityBehaviourDefinition behaviour;
    }
}
