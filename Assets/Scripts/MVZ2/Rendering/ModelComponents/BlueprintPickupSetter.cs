using MVZ2.GameContent;
using MVZ2.Managers;
using MVZ2.UI;
using PVZEngine;
using PVZEngine.Level;
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
                var main = MainManager.Instance;
                lastID = blueprintID;
                var resourceManager = main.ResourceManager;
                BlueprintViewData viewData = new BlueprintViewData()
                {
                    triggerActive = false,
                    cost = "0",
                    triggerCost = "0",
                    icon = resourceManager.GetDefaultSprite()
                };
                if (NamespaceID.IsValid(blueprintID))
                {
                    var definition = main.Game.GetSeedDefinition(blueprintID);
                    if (definition != null)
                    {
                        viewData.icon = resourceManager.GetBlueprintIcon(definition);
                        viewData.cost = definition.GetCost().ToString();
                        viewData.triggerCost = definition.GetTriggerCost().ToString();
                        viewData.triggerActive = definition.IsTriggerActive();
                    }
                }
                bool isMobile = main.IsMobile();
                var blueprintSprite = isMobile ? blueprintSpriteMobile : blueprintSpriteStandalone;
                blueprintSpriteStandalone.gameObject.SetActive(!isMobile);
                blueprintSpriteMobile.gameObject.SetActive(isMobile);
                blueprintSprite.UpdateView(viewData);

                colliderStandalone.SetActive(!isMobile);
                colliderMobile.SetActive(isMobile);
            }
        }
        [SerializeField]
        private GameObject colliderStandalone;
        [SerializeField]
        private GameObject colliderMobile;
        [SerializeField]
        private BlueprintSprite blueprintSpriteStandalone;
        [SerializeField]
        private BlueprintSprite blueprintSpriteMobile;
        private NamespaceID lastID;
    }
}
