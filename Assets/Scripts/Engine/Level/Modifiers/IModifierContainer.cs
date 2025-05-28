using PVZEngine.Entities;

namespace PVZEngine.Modifiers
{
    public interface IModifierContainer
    {
        public T GetProperty<T>(PropertyKey<T> name);
    }
}
