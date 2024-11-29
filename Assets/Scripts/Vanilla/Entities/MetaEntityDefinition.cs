using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
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
