using PVZEngine.Base;

namespace PVZEngine
{
    public interface IContentProvider
    {
        T GetDefinition<T>(NamespaceID defRef) where T : Definition;
        T[] GetDefinitions<T>() where T : Definition;
        Definition[] GetDefinitions();
    }
}
