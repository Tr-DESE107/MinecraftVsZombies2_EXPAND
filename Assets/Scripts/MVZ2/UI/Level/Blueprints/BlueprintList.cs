﻿using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class BlueprintList : ClassicBlueprintSet
    {
        public override Blueprint CreateBlueprint()
        {
            return blueprints.CreateItem().GetComponent<Blueprint>();
        }
        public override void InsertBlueprint(int index, Blueprint blueprint)
        {
            if (!blueprint)
                return;
            blueprints.Insert(index, blueprint.gameObject);
            blueprint.Index = index;
            blueprint.IsInConveyor = false;
            blueprint.OnPointerInteraction += OnBlueprintPointerInteractionCallback;
        }
        public override bool RemoveBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (blueprints.Remove(blueprint.gameObject))
            {
                blueprint.Index = -1;
                blueprint.IsInConveyor = false;
                blueprint.OnPointerInteraction -= OnBlueprintPointerInteractionCallback;
                return true;
            }
            return false;
        }
        public override bool DestroyBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (blueprints.DestroyItem(blueprint.gameObject))
            {
                blueprint.Index = -1;
                blueprint.IsInConveyor = false;
                blueprint.OnPointerInteraction -= OnBlueprintPointerInteractionCallback;
                return true;
            }
            return false;
        }
        public override Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
        }
        public override int GetBlueprintIndex(Blueprint value)
        {
            return blueprints.indexOf(value);
        }
        public Vector3 GetBlueprintPosition(int index)
        {
            var local = GetBlueprintLocalPosition(index);
            return blueprints.ListRoot.TransformPoint(local);
        }
        public override int GetBlueprintCount()
        {
            return blueprints.Count;
        }
        [SerializeField]
        private ElementList blueprints;
    }
}
