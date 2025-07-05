using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Entities;
using PVZEngine.Level.Collisions;

namespace MVZ2Logic.Entities
{
    public class MetaArmorDefinition : ArmorDefinition
    {
        public MetaArmorDefinition(string nsp, string name, ColliderConstructor[] constructors) : base(nsp, name, constructors)
        {
        }
        public override IEnumerable<ColliderConstructor> GetColliderConstructors(Entity entity, NamespaceID slotID)
        {
            var armorID = GetID();
            var position = entity.GetArmorOffset(slotID, armorID);
            var scale = entity.GetArmorScale(slotID, armorID);
            foreach (var cons in base.GetColliderConstructors(entity, slotID))
            {
                var newCons = cons;
                newCons.offset.Scale(scale);
                newCons.offset += position;
                newCons.size.Scale(scale);
                yield return newCons;
            }
        }
    }
}
