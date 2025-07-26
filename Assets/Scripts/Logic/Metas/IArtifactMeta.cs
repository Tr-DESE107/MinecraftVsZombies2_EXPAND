using System;
using PVZEngine;

namespace MVZ2Logic.Entities
{
    public interface IArtifactMeta
    {
        string ID { get; }
        string Name { get; }
        string Tooltip { get; }
        [Obsolete]
        NamespaceID Unlock { get; }
        SpriteReference Sprite { get; }
        int Order { get; }
    }
}
