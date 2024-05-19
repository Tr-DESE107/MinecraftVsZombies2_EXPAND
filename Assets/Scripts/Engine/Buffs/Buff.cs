using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVZEngine
{
    public class Buff
    {
        public Buff(BuffDefinition definition)
        {
            Definition = definition;
        }
        public object ModifyProperty(object property)
        {
            foreach (var modi in GetModifiers())
            {
                property = modi.CalculateProperty(Entity, this, property);
            }
            return property;
        }
        public Modifier[] GetModifiers()
        {
            return Definition.GetModifiers();
        }
        public void AddToEntity(Entity entity)
        {
            if (Entity != null)
                return;
            Entity = entity;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostAdd(entity, this);
            }
        }
        public void RemoveFromEntity()
        {
            if (Entity == null)
                return;
            foreach (var modifier in GetModifiers())
            {
                modifier.PostRemove(Entity, this);
            }
            Entity = null;
        }
        public BuffDefinition Definition { get; }
        public Entity Entity { get; private set; }
    }
}
