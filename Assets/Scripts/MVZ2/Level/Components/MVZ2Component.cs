﻿using MVZ2.Managers;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public interface IMVZ2LevelComponent
    {
        void PostLevelLoad();
        void UpdateFrame(float deltaTime, float simulationSpeed);
        void PostDispose();
    }
    public abstract class MVZ2Component : LevelComponent, IMVZ2LevelComponent
    {
        public MVZ2Component(LevelEngine level, NamespaceID id, LevelController controller) : base(level, id)
        {
            Controller = controller;
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new EmptySerializableLevelComponent();
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
        }
        public virtual void PostLevelLoad()
        {

        }
        public virtual void PostDispose()
        {

        }
        public virtual void UpdateFrame(float deltaTime, float simulationSpeed)
        {

        }
        protected MainManager Main => MainManager.Instance;
        public LevelController Controller { get; private set; }
    }
}
