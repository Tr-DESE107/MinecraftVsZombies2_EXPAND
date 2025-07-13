using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Supporters;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.Level
{
    public partial class LevelController
    {
        private void AddLevelCallbacks_Sponsors(LevelEngine level)
        {
            level.AddTrigger(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, EnginePostUseEntityBlueprintCallback);
        }
        private void EnginePostUseEntityBlueprintCallback(VanillaLevelCallbacks.PostUseEntityBlueprintParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            var seed = param.blueprint;
            var definition = param.definition;
            var heldData = param.heldData;
            if (!Main.OptionsManager.ShowSponsorNames())
                return;
            if (entity.IsEntityOf(VanillaContraptionID.furnace))
            {
                ShowFurnaceSponsorName(entity);
            }
            else if (entity.IsEntityOf(VanillaContraptionID.moonlightSensor))
            {
                ShowMoonlightSensorSponsorName(entity);
            }
        }
        private void ShowFurnaceSponsorName(Entity furnace)
        {
            var names = Main.SponsorManager.GetSponsorPlanNames(SponsorPlans.Furnace.TYPE, SponsorPlans.Furnace.FURNACE);
            if (names.Length <= 0)
                return;
            var text = furnace.Spawn(VanillaEffectID.floatingText, furnace.GetCenter(), rng.Next());
            var name = names.Random(text.RNG);
            FloatingText.SetText(text, name);
        }
        private void ShowMoonlightSensorSponsorName(Entity sensor)
        {
            var names = Main.SponsorManager.GetSponsorPlanNames(SponsorPlans.Sensor.TYPE, SponsorPlans.Sensor.MOONLIGHT_SENSOR);
            if (names.Length <= 0)
                return;
            var text = sensor.Spawn(VanillaEffectID.floatingText, sensor.GetCenter(), rng.Next());
            var name = names.Random(text.RNG);
            FloatingText.SetText(text, name);
        }
    }
}
