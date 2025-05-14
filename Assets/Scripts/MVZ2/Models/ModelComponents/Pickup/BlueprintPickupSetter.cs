using MVZ2.Managers;
using MVZ2.UI;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Models
{
    public class BlueprintPickupSetter : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            UpdateBlueprint();
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            UpdateBlueprint();
        }
        private void UpdateBlueprint()
        {
            var main = MainManager.Instance;
            var resourceManager = main.ResourceManager;
            bool isMobile = main.IsMobile();
            var blueprintID = Model.GetProperty<NamespaceID>("BlueprintID");
            var commandBlock = Model.GetProperty<bool>("CommandBlock");
            if (lastID != blueprintID || lastCommandBlock != commandBlock)
            {
                lastID = blueprintID;
                lastCommandBlock = commandBlock;
                BlueprintViewData viewData = resourceManager.GetBlueprintViewData(blueprintID, false, commandBlock);
                var blueprintSprite = isMobile ? blueprintSpriteMobile : blueprintSpriteStandalone;
                blueprintSprite.UpdateView(viewData);
            }
            blueprintSpriteStandalone.gameObject.SetActive(!isMobile);
            blueprintSpriteMobile.gameObject.SetActive(isMobile);
            colliderStandalone.SetActive(!isMobile);
            colliderMobile.SetActive(isMobile);
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
        private bool lastCommandBlock;
    }
}
