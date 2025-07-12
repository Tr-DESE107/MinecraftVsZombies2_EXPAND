using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.commandBlock)]
    public class CommandBlock : ContraptionBehaviour
    {
        public CommandBlock(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.CAN_CONTRAPTION_SACRIFICE, CanContraptionSacrificeCallback, filter: VanillaContraptionID.commandBlock);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(IDLE_TIME));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var stateTimer = GetStateTimer(entity);
            if (stateTimer == null)
            {
                TransformBlock(entity);
                return;
            }
            stateTimer.Run();
            if (stateTimer.Expired)
            {
                if (entity.State == STATE_IDLE)
                {
                    stateTimer.ResetTime(WORK_TIME);
                    entity.State = STATE_WORKING;
                    entity.PlaySound(VanillaSoundID.dataStream);
                }
                else if (entity.State == STATE_WORKING)
                {
                    TransformBlock(entity);
                }
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            bool frozen = entity.IsAIFrozen();
            entity.SetAnimationBool("Working", !frozen && entity.State == STATE_WORKING);
        }
        public override bool CanTrigger(Entity entity)
        {
            if (IsTriggered(entity))
                return false;
            var targetDef = GetEntityDefinitionToTransform(entity);
            if (targetDef != null && targetDef.CanInstantTrigger())
                return true;
            return base.CanTrigger(entity);
        }
        public override bool CanEvoke(Entity entity)
        {
            var targetDef = GetEntityDefinitionToTransform(entity);
            if (targetDef == null)
                return false;
            if (!targetDef.WillInstantEvoke(entity.Level))
                return false;
            return base.CanEvoke(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
        }
        public override void Trigger(Entity entity)
        {
            base.Trigger(entity);
            SetTriggered(entity, true);
            entity.PlaySound(VanillaSoundID.wakeup);
            WhiteFlashBuff.AddToEntity(entity, 15);
        }
        public static void TransformBlock(Entity entity)
        {
            var targetID = GetEntityDefinitionToTransform(entity);
            if (targetID == null)
            {
                entity.Spawn(VanillaContraptionID.errorBlock, entity.Position);
                entity.PlaySound(VanillaSoundID.errorXP);
                entity.Remove();
                return;
            }
            var grid = entity.GetGrid();
            var spawnParams = entity.GetSpawnParams();
            spawnParams.SetProperty(VanillaEntityProps.VARIANT, entity.GetVariant());
            var spawned = grid.SpawnPlacedEntity(targetID.GetID(), spawnParams);
            spawned.AddBuff<ImitatedBuff>();
            if (IsTriggered(entity) && spawned.CanTrigger())
            {
                spawned.Trigger();
            }
            if (entity.IsEvoked() && spawned.CanEvoke())
            {
                spawned.Evoke();
            }
            entity.Spawn(VanillaEffectID.binaryParticles, entity.GetCenter());

            entity.Remove();
        }
        private static EntityDefinition GetEntityDefinitionToTransform(Entity entity)
        {
            var targetEntity = GetTargetEntity(entity);
            if (targetEntity == null)
                return null;
            var targetDef = entity.Level.Content.GetEntityDefinition(targetEntity);
            if (targetDef == null)
                return null;
            return targetDef;
        }
        public static SpawnParams GetImitateSpawnParams(NamespaceID target)
        {
            var spawnParam = new SpawnParams();
            spawnParam.SetProperty(PROP_TARGET_ENTITY, target);
            var definition = Global.Game.GetEntityDefinition(target);
            if (definition != null)
            {
                spawnParam.SetProperty(VanillaEntityProps.WATER_INTERACTION, definition.GetWaterInteraction());
                spawnParam.SetProperty(VanillaEntityProps.AIR_INTERACTION, definition.GetAirInteraction());
                spawnParam.SetProperty(VanillaEntityProps.GRID_LAYERS, definition.GetGridLayersToTake());
            }
            return spawnParam;
        }
        private void CanContraptionSacrificeCallback(VanillaLevelCallbacks.ContraptionSacrificeValueParams param, CallbackResult result)
        {
            result.SetFinalValue(false);
        }
        public static bool IsTriggered(Entity entity) => entity.GetBehaviourField<bool>(PROP_TRIGGERED);
        public static void SetTriggered(Entity entity, bool timer) => entity.SetBehaviourField(PROP_TRIGGERED, timer);
        public static FrameTimer GetStateTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_PRODUCTION_TIMER);
        public static void SetStateTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_PRODUCTION_TIMER, timer);
        public static NamespaceID GetTargetEntity(Entity entity) => entity.GetBehaviourField<NamespaceID>(PROP_TARGET_ENTITY);
        public static void SetTargetEntity(Entity entity, NamespaceID value) => entity.SetBehaviourField(PROP_TARGET_ENTITY, value);
        public const int IDLE_TIME = 69;
        public const int WORK_TIME = 27;
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        public const int STATE_WORKING = VanillaEntityStates.CONTRAPTION_SPECIAL;

        private static readonly VanillaEntityPropertyMeta<bool> PROP_TRIGGERED = new VanillaEntityPropertyMeta<bool>("triggered");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
        private static readonly VanillaEntityPropertyMeta<NamespaceID> PROP_TARGET_ENTITY = new VanillaEntityPropertyMeta<NamespaceID>("TargetEntity");
    }
}
