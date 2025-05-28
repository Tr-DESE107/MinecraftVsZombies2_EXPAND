﻿using System.Reflection;
using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;

namespace MVZ2Logic.Modding
{
    public interface IModLogic : IGameContent
    {
        void Init(IGame game, Assembly[] assemblies);
        void LateInit(IGame game);
        void PostReloadMods(IGame game);
        void PostGameInit();
        void Load();
        void Unload();
        ModSaveData CreateSaveData();
        ModSaveData LoadSaveData(string json);
    }
}
