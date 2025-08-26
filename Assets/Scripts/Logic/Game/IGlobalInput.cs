using UnityEngine;

namespace MVZ2Logic.Games
{
    public interface IGlobalInput
    {
        Vector2 GetPointerScreenPosition();
        bool IsPointerDown(int type, int button);
        bool IsPointerHolding(int type, int button);
        bool IsPointerUp(int type, int button);
    }
}
