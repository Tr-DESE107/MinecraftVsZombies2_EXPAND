using PVZEngine;
using UnityEngine;

namespace MVZ2Logic.Map
{
    public interface IMapMeta
    {
        string ID { get; }
        NamespaceID Area { get; }
        NamespaceID EndlessUnlock { get; }
        IMapPreset[] Presets { get; }
        NamespaceID[] Stages { get; }
    }
    public class IMapPreset
    {
        NamespaceID ID { get; }
        NamespaceID Model { get; }
        NamespaceID Music { get; }
        Color BackgroundColor { get; }
    }
}
