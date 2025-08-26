﻿using System;
using MVZ2.Managers;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level
{
    public interface ILevelControllerPart
    {
        void Init(LevelController controller);
        void AddEngineCallbacks(LevelEngine level);
        void RemoveEngineCallbacks(LevelEngine level);
        void PostLevelStart();
        void UpdateLogic();
        void UpdateFrame(float deltaTime, float simulationSpeed);
        void PostLevelLoad();
        SerializableLevelControllerPart ToSerializable();
        void LoadFromSerializable(SerializableLevelControllerPart seri);
        public NamespaceID ID { get; }

    }
    public abstract class LevelControllerPart : MonoBehaviour, ILevelControllerPart
    {
        public virtual void Init(LevelController controller)
        {
            Controller = controller;
        }
        public virtual void AddEngineCallbacks(LevelEngine level) { }
        public virtual void RemoveEngineCallbacks(LevelEngine level) { }
        public virtual void PostLevelStart() { }
        public virtual void UpdateLogic() { }
        public virtual void UpdateFrame(float deltaTime, float simulationSpeed) { }
        public virtual void PostLevelLoad() { }
        public SerializableLevelControllerPart ToSerializable()
        {
            var seri = GetSerializable();
            seri.id = ID;
            return seri;
        }
        protected abstract SerializableLevelControllerPart GetSerializable();
        public virtual void LoadFromSerializable(SerializableLevelControllerPart seri) { }
        public IGlobalGame Game => Controller.Game;
        public ILevelUI UI => Controller.GetUI();
        public MainManager Main => MainManager.Instance;
        public LevelEngine Level => Controller.GetEngine();
        public ILevelController Controller { get; private set; }
        public NamespaceID ID => id.Get();
        [SerializeField]
        private NamespaceIDReference id;
    }
    [Serializable]
    public abstract class SerializableLevelControllerPart
    {
        public NamespaceID id;
    }
}
