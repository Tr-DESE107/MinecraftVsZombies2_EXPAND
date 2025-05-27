using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Level.Components
{
    public abstract class UnityLevelComponent : MonoBehaviour, ILevelComponent, IMVZ2LevelComponent
    {
        public virtual void PostAttach(LevelEngine level) { }
        public virtual void PostDetach(LevelEngine level) { }
        public virtual ISerializableLevelComponent ToSerializable()
        {
            return new EmptySerializableLevelComponent();
        }
        public virtual void LoadSerializable(ISerializableLevelComponent seri)
        {
        }
        public virtual void OnStart()
        {

        }
        public virtual void OnUpdate()
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
        void ILevelComponent.Update() => OnUpdate();
        public NamespaceID GetID()
        {
            return id.Get();
        }
        public LevelEngine Level => Controller.GetEngine();
        public LevelController Controller => levelController;
        [SerializeField]
        private LevelController levelController;
        [SerializeField]
        private NamespaceIDReference id;
    }
}
