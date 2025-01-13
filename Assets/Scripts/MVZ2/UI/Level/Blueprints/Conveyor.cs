using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class Conveyor : BlueprintSet
    {
        public void SetSlotCount(int count)
        {
            slotCount = count;
            structureList.updateList(count);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
        public override Blueprint CreateBlueprint()
        {
            return blueprints.CreateItem().GetComponent<Blueprint>();
        }
        public override void InsertBlueprint(int index, Blueprint blueprint)
        {
            if (!blueprint)
                return;
            blueprints.Insert(index, blueprint.gameObject);
            blueprint.OnPointerEnter += OnBlueprintPointerEnterCallback;
            blueprint.OnPointerExit += OnBlueprintPointerExitCallback;
            blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
        }
        public override bool RemoveBlueprint(Blueprint blueprint)
        {
            if (!blueprint)
                return false;
            if (blueprints.Remove(blueprint.gameObject))
            {
                blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
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
                blueprint.OnPointerEnter -= OnBlueprintPointerEnterCallback;
                blueprint.OnPointerExit -= OnBlueprintPointerExitCallback;
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
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
        public void SetBlueprintNormalizedPosition(int index, float position)
        {
            Blueprint blueprint = GetBlueprintAt(index);
            if (blueprint == null)
                return;
            blueprint.transform.position = Vector3.LerpUnclamped(startPositionAnchor.position, endPositionAnchor.position, position / (slotCount - 1));
        }
        private int slotCount;
        [SerializeField]
        private ElementList blueprints;
        [SerializeField]
        private ElementList structureList;
        [SerializeField]
        private Transform startPositionAnchor;
        [SerializeField]
        private Transform endPositionAnchor;
    }
}
