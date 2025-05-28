using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.nightmareaperFall)]
    public class NightmareaperFallBuff : BuffDefinition
    {
        public NightmareaperFallBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(VanillaEntityProps.WATER_INTERACTION, NumberOperator.Set, WaterInteraction.REMOVE));
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Add, 1));
            AddTrigger(VanillaLevelCallbacks.POST_WATER_INTERACTION, PostWaterInteractionCallback, filter: WaterInteraction.ACTION_REMOVE);
        }
        private void PostWaterInteractionCallback(VanillaLevelCallbacks.WaterInteractionParams param, CallbackResult callbackResult)
        {
            var entity = param.entity;
            if (!entity.HasBuff<NightmareaperFallBuff>())
                return;
            entity.Spawn(VanillaEffectID.nightmareaperSplash, entity.Position);
        }
    }
}
