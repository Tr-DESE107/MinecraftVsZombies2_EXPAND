using PVZEngine.Armors;
using PVZEngine.Level.Collisions;

namespace MVZ2Logic.Entities
{
    public class MetaArmorDefinition : ArmorDefinition
    {
        public MetaArmorDefinition(string nsp, string name, ColliderConstructor[] constructors) : base(nsp, name, constructors)
        {
        }
    }
}
