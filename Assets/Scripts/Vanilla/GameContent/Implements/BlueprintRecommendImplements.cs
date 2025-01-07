using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Modding;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Implements
{
    public class BlueprintRecommendImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LogicLevelCallbacks.GET_BLUEPRINT_NOT_RECOMMONDED, GetBlueprintNotRecommondedCallback);
        }

        private void GetBlueprintNotRecommondedCallback(LevelEngine level, NamespaceID blueprint, TriggerResultBoolean result)
        {
            var content = level.Content;
            var blueprintDef = content.GetSeedDefinition(blueprint);
            if (blueprintDef == null)
                return;

            if (level.IsDay())
            {
                if (blueprintDef.GetSeedType() == SeedTypes.ENTITY)
                {
                    var entityID = blueprintDef.GetSeedEntityID();
                    if (entityID == VanillaContraptionID.glowstone)
                    {
                        result.Result = true;
                        return;
                    }
                    var entityDef = content.GetEntityDefinition(entityID);
                    if (entityDef != null && entityDef.IsNocturnal())
                    {
                        result.Result = true;
                        return;
                    }
                }
            }
        }
    }
}
