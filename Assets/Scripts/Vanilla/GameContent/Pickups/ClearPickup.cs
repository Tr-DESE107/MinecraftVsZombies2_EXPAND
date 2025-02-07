using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.clearPickup)]
    public class ClearPickup : PickupBehaviour
    {
        public ClearPickup(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var modelID = GetPickupModelID(entity);
            entity.ChangeModel(modelID);

            var level = entity.Level;
            if (modelID == VanillaModelID.blueprintPickup)
            {
                var blueprintID = level.GetClearPickupBlueprint();
                if (NamespaceID.IsValid(blueprintID))
                    entity.SetModelProperty("BlueprintID", blueprintID);
            }
            if (!level.IsRerun)
            {
                var noteID = level.GetEndNoteID();
                if (NamespaceID.IsValid(noteID))
                {
                    entity.SetProperty(VanillaPickupProps.REMOVE_ON_COLLECT, true);
                }
            }
            if (entity.ModelID == VanillaModelID.mapPickup)
            {
                entity.SetCollectSound(VanillaSoundID.pick);
            }
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float shadowAlpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = pickup.GetCollectedTime();
                var moveTime = level.GetSecondTicks(3);
                float timePercent = collectedTime / (float)moveTime;
                var targetPos = GetMoveTargetPosition(pickup);
                pickup.Velocity = (targetPos - pickup.Position) * 0.05f;
                pickup.SetScale(Vector3.one * Mathf.Lerp(1, 3, timePercent));
                pickup.SetDisplayScale(Vector3.one * Mathf.Lerp(1, 3, timePercent));

                shadowAlpha = 0;
            }
            pickup.SetShadowAlpha(shadowAlpha);
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            pickup.Velocity = Vector3.zero;
            var level = pickup.Level;
            if (level.IsRerun)
            {
                int money = 250;
                if (level.Difficulty == VanillaDifficulties.easy)
                {
                    money = 50;
                }
                else if (level.Difficulty == VanillaDifficulties.hard)
                {
                    money = 1000;
                }
                GemEffect.SpawnGemEffects(level, money, pickup.Position, pickup, false);
            }
            else
            {
                if (level.Difficulty == VanillaDifficulties.hard)
                {
                    GemEffect.SpawnGemEffects(level, 250, pickup.Position, pickup, false);
                }
            }
            foreach (var p in level.GetEntities(EntityTypes.PICKUP))
            {
                if (p == pickup || p.IsCollected())
                    continue;
                p.Collect();
            }
            level.Clear();
            level.StopMusic();
            level.PlaySound(pickup.GetCollectSound());
            level.PlaySound(pickup.Level.GetClearSound());
            level.Spawn(VanillaEffectID.starParticles, pickup.Position, pickup);

        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetScreenCenterPosition() + Vector2.down * (entity.GetSize().y * entity.GetDisplayScale().y * 0.5f);
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        private NamespaceID GetPickupModelID(Entity entity)
        {
            var level = entity.Level;
            if (level.IsRerun)
                return VanillaModelID.moneyChest;
            return level.GetClearPickupModel() ?? VanillaModelID.blueprintPickup;
        }
        private const float COLLECTED_Z = 0;
    }
}