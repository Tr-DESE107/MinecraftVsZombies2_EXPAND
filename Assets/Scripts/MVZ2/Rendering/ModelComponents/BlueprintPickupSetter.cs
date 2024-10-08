using MVZ2.GameContent;
using MVZ2.Managers;
using PVZEngine;
using PVZEngine.Level;
using TMPro;
using UnityEngine;

namespace MVZ2.Rendering
{
    public class BlueprintPickupSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var blueprintID = Model.GetProperty<NamespaceID>("BlueprintID");
            if (lastID != blueprintID)
            {
                lastID = blueprintID;
                if (blueprintID != null)
                {
                    var resourceManager = MainManager.Instance.ResourceManager;
                    var entityID = blueprintID;
                    var modelID = entityID.ToModelID(EngineModelID.TYPE_ENTITY);
                    var modelIcon = resourceManager.GetModelIcon(modelID) ?? resourceManager.GetDefaultSprite();
                    iconSprite.sprite = modelIcon;

                    var spriteScale = 64f / Mathf.Max(modelIcon.rect.width, modelIcon.rect.height);
                    iconSprite.transform.localScale = Vector3.one * spriteScale;

                    var definition = MainManager.Instance.Game.GetSeedDefinition(blueprintID);
                    if (definition == null)
                    {
                        costText.text = "0";
                        triggerCostText.text = "0";
                        triggerCostText.gameObject.SetActive(false);
                    }
                    else
                    {
                        costText.text = definition.GetCost().ToString();
                        triggerCostText.text = definition.GetTriggerCost().ToString();
                        triggerCostText.gameObject.SetActive(definition.IsTriggerActive());
                    }
                }
                else
                {
                    iconSprite.sprite = null;
                    costText.text = null;
                    triggerCostText.text = null;
                    triggerCostText.gameObject.SetActive(false);
                }
            }
        }
        [SerializeField]
        private SpriteRenderer iconSprite;
        [SerializeField]
        private TextMeshPro costText;
        [SerializeField]
        private TextMeshPro triggerCostText;
        private NamespaceID lastID;
    }
}
