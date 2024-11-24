using PVZEngine;

namespace MVZ2Logic.Models
{
    public interface IModelMeta
    {
        string Name { get; }
        string Type { get; }
        NamespaceID Path { get; }
        int Width { get; }
        int Height { get; }
        float XOffset { get; }
        float YOffset { get; }
    }
}
