using PVZEngine;

namespace MVZ2Logic.Notes
{
    public interface INoteMeta
    {
        string ID { get; }
        SpriteReference Sprite { get; }
        SpriteReference Background { get; }
        NamespaceID StartTalk { get; }
        bool CanFlip { get; }
        SpriteReference FlipSprite { get; }
    }
}
