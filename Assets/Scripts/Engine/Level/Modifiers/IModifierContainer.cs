using PVZEngine.Entities;

namespace PVZEngine.Modifiers
{
    public interface IModifierContainer
    {
        public object GetProperty(PropertyKey name);
    }
}
