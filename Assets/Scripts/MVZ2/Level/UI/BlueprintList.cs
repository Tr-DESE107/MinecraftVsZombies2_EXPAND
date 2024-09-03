using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level.UI
{
    public class BlueprintList : LevelUIUnit
    {
        public void SetBlueprints(BlueprintViewData[] viewDatas)
        {
            blueprints.updateList(viewDatas.Length, (i, rect) =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                var viewData = viewDatas[i];
                blueprint.SetCost(viewData.cost);
                blueprint.SetIcon(viewData.icon);
                blueprint.SetTriggerActive(viewData.triggerActive);
                blueprint.SetTriggerCost(viewData.triggerCost);
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerDown += OnBlueprintPointerDownCallback;
            },
            rect =>
            {
                var blueprint = rect.GetComponent<Blueprint>();
                blueprint.OnPointerDown -= OnBlueprintPointerDownCallback;
            });
        }
        public void SetRecharges(float[] recharges)
        {
            for (int i = 0; i < blueprints.count; i++)
            {
                var recharge = recharges[i];
                var blueprint = blueprints.getElement<Blueprint>(i);
                if (!blueprint)
                    continue;
                blueprint.SetRecharge(1 - recharge);
            }
        }
        public void SetDisabled(bool[] disabledList)
        {
            for (int i = 0; i < blueprints.count; i++)
            {
                var disabled = disabledList[i];
                var blueprint = blueprints.getElement<Blueprint>(i);
                if (!blueprint)
                    continue;
                blueprint.SetDisabled(disabled);
            }
        }
        public void SetBlueprintCount(int count)
        {
            blueprintPlaceholders.updateList(count);
        }
        private void OnBlueprintPointerDownCallback(Blueprint blueprint, PointerEventData data)
        {
            OnBlueprintPointerDown?.Invoke(blueprints.indexOf(blueprint), data);
        }
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        private ElementList blueprints;
        [SerializeField]
        private ElementList blueprintPlaceholders;
    }
}
