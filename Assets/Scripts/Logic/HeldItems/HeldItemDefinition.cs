﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.HeldItems;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemDefinition : Definition, ICachedDefinition
    {
        public HeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        protected void AddBehaviour(NamespaceID behaviour)
        {
            behaviours.Add(behaviour);
        }
        public bool HasBehaviour(LevelEngine level, IHeldItemData data, HeldItemBehaviourDefinition behaviour)
        {
            return GetBehaviours()?.Contains(behaviour) ?? false;
        }
        public void Begin(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in GetBehaviours())
            {
                behaviour.OnBegin(level, data);
            }
        }
        public void End(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in GetBehaviours())
            {
                behaviour.OnEnd(level, data);
            }
        }
        public void Update(LevelEngine level, IHeldItemData data)
        {
            foreach (var behaviour in GetBehaviours())
            {
                behaviour.OnUpdate(level, data);
            }
        }

        public void CacheContents(IGameContent content)
        {
            foreach (var id in behaviours)
            {
                var behaviour = content.GetHeldItemBehaviourDefinition(id);
                if (behaviour == null)
                    continue;
                behavioursCache.Add(behaviour);
            }
        }

        public void ClearCaches()
        {
            behavioursCache.Clear();
        }
        public bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            foreach (var behaviour in GetBehaviours())
            {
                if (behaviour.IsValidFor(target, data, interactionData))
                    return true;
            }
            return false;
        }
        public HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerData pointer)
        {
            var interactionData = new PointerInteractionData()
            {
                pointer = pointer,
                interaction = PointerInteraction.Hover
            };
            foreach (var behaviour in GetBehaviours())
            {
                if (behaviour.IsValidFor(target, data, interactionData))
                {
                    var highlight = behaviour.GetHighlight(target, data, interactionData);
                    if (highlight.mode != HeldHighlightMode.None)
                        return highlight;
                }
            }
            return HeldHighlight.None;
        }
        public void DoPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            foreach (var behaviour in GetBehaviours())
            {
                if (behaviour.IsValidFor(target, data, pointerParams))
                {
                    behaviour.OnPointerEvent(target, data, pointerParams);
                }
            }
        }
        public void PostSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
            foreach (var behaviour in GetBehaviours())
            {
                behaviour.OnSetModel(level, data, model);
            }
        }
        public NamespaceID GetModelID(LevelEngine level, IHeldItemData data)
        {
            var callbackResult = new CallbackResult(null);
            foreach (var behaviour in GetBehaviours())
            {
                if (callbackResult.IsBreakRequested)
                    break;
                behaviour.GetModelID(level, data, callbackResult);
            }
            return callbackResult.GetValue<NamespaceID>();
        }
        public float GetRadius(LevelEngine level, IHeldItemData data)
        {
            var callbackResult = new CallbackResult(0);
            foreach (var behaviour in GetBehaviours())
            {
                if (callbackResult.IsBreakRequested)
                    break;
                behaviour.GetRadius(level, data, callbackResult);
            }
            return callbackResult.GetValue<float>();
        }
        public T GetBehaviour<T>()
        {
            return behavioursCache.OfType<T>().FirstOrDefault();
        }
        public IEnumerable<HeldItemBehaviourDefinition> GetBehaviours()
        {
            return behavioursCache;
        }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.HELD_ITEM;

        private List<NamespaceID> behaviours = new List<NamespaceID>();
        private List<HeldItemBehaviourDefinition> behavioursCache = new List<HeldItemBehaviourDefinition>();
    }
    public enum LawnArea
    {
        Side,
        Main,
        Bottom
    }
}
