using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using static UnityEngine.GraphicsBuffer;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.entity)]
    public class EntityHeldItemDefinition : HeldItemDefinition
    {
        public EntityHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool CheckRaycast(HeldItemTarget target, IHeldItemData data)
        {
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return false;
            return behaviour.CheckRaycast(entity, target, data);
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return HeldHighlight.None;
            return behaviour.GetHighlight(entity, target, data);
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerPhase phase)
        {
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return;
            behaviour.Use(entity, target, data, phase);
        }
        public override SeedPack GetSeedPack(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return null;
            return behaviour.GetSeedPack(entity, level, data);
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return null;
            return behaviour.GetModelID(entity, level, data);
        }
        public override float GetRadius(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return 0;
            return behaviour.GetRadius(entity, level, data);
        }
        public override void Update(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            if (behaviour == null)
                return;
            behaviour.Update(entity, level, data);
        }
        public Entity GetEntity(LevelEngine level, long id)
        {
            return level.FindEntityByID(id);
        }
        public static IEntityHeldItemBehaviour GetBehaviour(Entity entity)
        {
            if (entity == null)
                return null;
            return entity.Definition.GetBehaviour<IEntityHeldItemBehaviour>();
        }
    }

    public interface IEntityHeldItemBehaviour
    {
        bool CheckRaycast(Entity entity, HeldItemTarget target, IHeldItemData data);
        HeldHighlight GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data);
        void Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerPhase phase);
        SeedPack GetSeedPack(Entity entity, LevelEngine level, IHeldItemData data);
        NamespaceID GetModelID(Entity entity, LevelEngine level, IHeldItemData data);
        float GetRadius(Entity entity, LevelEngine level, IHeldItemData data);
        void Update(Entity entity, LevelEngine level, IHeldItemData data);
    }
}
