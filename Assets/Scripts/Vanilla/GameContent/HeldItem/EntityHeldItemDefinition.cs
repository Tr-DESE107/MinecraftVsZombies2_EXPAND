using System;
using MVZ2.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemDefinition(VanillaHeldItemNames.entity)]
    public class EntityHeldItemDefinition : HeldItemDefinition
    {
        public EntityHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviours = GetBehaviours(entity);
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsHeldItemValidFor(entity, target, data, interactionData))
                    return true;
            }
            return false;
        }
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviours = GetBehaviours(entity);
            foreach (var behaviour in behaviours)
            {
                if (behaviour.IsHeldItemValidFor(entity, target, data, interactionData))
                {
                    return behaviour.GetHeldItemHighlight(entity, target, data, interactionData);
                }
            }
            return HeldHighlight.None;
        }
        public override void Begin(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntityBeginHandler>(entity);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnHeldItemBegin(entity, level, data);
            }
        }
        public override void End(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntityEndHandler>(entity);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnHeldItemEnd(entity, level, data);
            }
        }
        public override void Update(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntityUpdateHandler>(entity);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnHeldItemUpdate(entity, level, data);
            }
        }
        public override void DoPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var entity = GetEntity(target.GetLevel(), data.ID);
            var behaviours = GetBehaviours<IHeldEntityPointerEventHandler>(entity);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnHeldItemPointerEvent(entity, target, data, pointerParams);
            }
        }
        public override void PostSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            base.PostSetModel(level, data, model);
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntitySetModelHandler>(entity);
            foreach (var behaviour in behaviours)
            {
                behaviour.OnHeldItemSetModel(entity, level, data, model);
            }
        }
        public override NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntityGetModelIDHandler>(entity);
            var callbackResult = new CallbackResult(null);
            foreach (var behaviour in behaviours)
            {
                behaviour.GetHeldItemModelID(entity, level, data, callbackResult);
            }
            return callbackResult.GetValue<NamespaceID>();
        }
        public override float GetRadius(LevelEngine level, IHeldItemData data)
        {
            var entity = GetEntity(level, data.ID);
            var behaviours = GetBehaviours<IHeldEntityGetRadiusHandler>(entity);
            var callbackResult = new CallbackResult(0);
            foreach (var behaviour in behaviours)
            {
                behaviour.GetHeldItemRadius(entity, level, data, callbackResult);
            }
            return callbackResult.GetValue<float>();
        }
        public static Entity GetEntity(LevelEngine level, long id)
        {
            return level.FindEntityByID(id);
        }
        public static IHeldEntityBehaviour GetBehaviour(Entity entity)
        {
            return GetBehaviour<IHeldEntityBehaviour>(entity);
        }
        public static IHeldEntityBehaviour[] GetBehaviours(Entity entity)
        {
            return GetBehaviours<IHeldEntityBehaviour>(entity);
        }
        public static T GetBehaviour<T>(Entity entity)
        {
            if (entity == null)
                return default;
            return entity.Definition.GetBehaviour<T>();
        }
        public static T[] GetBehaviours<T>(Entity entity)
        {
            if (entity == null)
                return Array.Empty<T>();
            return entity.Definition.GetBehaviours<T>();
        }
    }
}
