﻿using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Level.UI
{
    public class BlueprintArray : ClassicBlueprintSet
    {
        public override void SetSlotCount(int count)
        {
            base.SetSlotCount(count);
            blueprints.SetCount(count);
        }
        public override Blueprint CreateBlueprint()
        {
            return blueprints.CreateItem().GetComponent<Blueprint>();
        }
        public override void InsertBlueprint(int index, Blueprint blueprint)
        {
            if (!blueprint)
                return;
            if (index < 0 || index >= blueprints.Count)
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
            if (!blueprints.Remove(blueprint.gameObject))
                return false;
            blueprint.Index = -1;
            blueprint.IsInConveyor = false;
            blueprint.OnPointerInteraction -= OnBlueprintPointerInteractionCallback;
            return true;
        }
        public override bool DestroyBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (!blueprints.DestroyItem(blueprint.gameObject))
                return false;
            blueprint.Index = -1;
            blueprint.IsInConveyor = false;
            blueprint.OnPointerInteraction -= OnBlueprintPointerInteractionCallback;
            return true;
        }
        public override Blueprint GetBlueprintAt(int index)
        {
            return blueprints.getElement<Blueprint>(index);
        }
        public override int GetBlueprintIndex(Blueprint value)
        {
            return blueprints.indexOf(value);
        }
        public void ForceAlign(int index)
        {
            var element = GetBlueprintAt(index);
            if (!element)
                return;
            element.transform.localPosition = GetBlueprintLocalPosition(index);
        }
        public Vector3 GetBlueprintPosition(int index)
        {
            var local = GetBlueprintLocalPosition(index);
            return blueprints.ListRoot.TransformPoint(local);
        }
        public void AlignRemainBlueprints(int removeIndex)
        {
            for (int i = removeIndex; i < blueprints.Count; i++)
            {
                if (i >= blueprints.Count - 1)
                    continue;
                var nextBlueprint = GetBlueprintAt(i + 1);
                RemoveBlueprintAt(i + 1);
                InsertBlueprint(i, nextBlueprint);
            }
        }
        public override int GetBlueprintCount()
        {
            return blueprints.Count;
        }
        [SerializeField]
        private ElementArray blueprints;
    }
}
