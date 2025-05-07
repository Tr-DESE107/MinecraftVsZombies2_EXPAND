using MVZ2.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.entity)]
    public class EntityHeldItemDefinition : HeldItemDefinition
    {
        public EntityHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
            AddBehaviour(new EntityHeldItemBehaviour(this));
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            return behaviour?.GetModelID(entity, level, data);
        }
        public override float GetRadius(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviour = GetBehaviour(entity);
            return behaviour?.GetRadius(entity, level, data) ?? 0;
        }
        public static Entity GetEntity(LevelEngine level, long id)
        {
            return level.FindEntityByID(id);
        }
        public static IHeldEntityBehaviour GetBehaviour(Entity entity)
        {
            if (entity == null)
                return null;
            return entity.Definition.GetBehaviour<IHeldEntityBehaviour>();
        }
    }

    public interface IHeldEntityBehaviour
    {
        bool IsValidFor(Entity entity, HeldItemTarget target, IHeldItemData data);
        HeldHighlight GetHighlight(Entity entity, HeldItemTarget target, IHeldItemData data);
        void Use(Entity entity, HeldItemTarget target, IHeldItemData data, PointerInteraction interaction);
        NamespaceID GetModelID(Entity entity, LevelEngine level, IHeldItemData data);
        float GetRadius(Entity entity, LevelEngine level, IHeldItemData data);
        void Update(Entity entity, LevelEngine level, IHeldItemData data);
    }
}
