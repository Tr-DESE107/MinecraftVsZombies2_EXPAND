using System.Collections;
using PVZEngine;
using UnityEngine;

namespace MVZ2Logic.Games
{
    public interface IGame : IGameContent, IGameLocalization, IGameTriggerSystem
    {
        bool IsMobile();
        Coroutine StartCoroutine(IEnumerator coroutine);
        string DefaultNamespace { get; }
    }
}
