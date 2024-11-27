using MVZ2.Managers;
using MVZ2.UI;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Models
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
                BlueprintViewData viewData = resourceManager.GetBlueprintViewData(blueprintID);
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
