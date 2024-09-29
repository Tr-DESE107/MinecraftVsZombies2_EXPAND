using MVZ2.Extensions;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent
{
    [Definition(PickupNames.clearPickup)]
    public class ClearPickup : VanillaPickup
    {
        public ClearPickup(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinPickupProps.IMPORTANT, true);
            SetProperty(PickupProps.COLLECT_SOUND, SoundID.winMusic);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var modelID = GetPickupModelID(entity);
            entity.ChangeModel(modelID);

            var level = entity.Level;
            if (modelID == VanillaModelID.blueprintPickup)
            {
                var game = Global.Game;
                var stageMeta = game.GetStageMeta(level.StageID);
                var blueprintID = stageMeta?.clearPickupBlueprint;
                if (NamespaceID.IsValid(blueprintID))
                    entity.SetModelProperty("BlueprintID", blueprintID);
            }
            var noteID = level.GetEndNoteID();
            if (NamespaceID.IsValid(noteID))
            {
                entity.SetProperty(PickupProps.REMOVE_ON_COLLECT, true);
            }
        }
        public override NamespaceID GetModelID()
        {
            return VanillaModelID.moneyChest;
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var level = pickup.Level;
            float shadowAlpha = 1;
            if (pickup.IsCollected())
            {
                var collectedTime = BuiltinPickup.GetCollectedTime(pickup);
                var moveTime = level.GetSecondTicks(3);
                float timePercent = collectedTime / (float)moveTime;
                var targetPos = GetMoveTargetPosition(pickup);
                pickup.Velocity = (targetPos - pickup.Pos) * 0.05f;
                pickup.Scale = Vector3.one * Mathf.Lerp(1, 3, timePercent);
                pickup.RenderScale = Vector3.one * Mathf.Lerp(1, 3, timePercent);

                shadowAlpha = 0;
            }
            pickup.SetShadowAlpha(shadowAlpha);
        }
        public override void PostContactGround(Entity entity)
        {
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
                if (level.Difficulty == LevelDifficulty.easy)
                {
                    money = 50;
                }
                else if (level.Difficulty == LevelDifficulty.hard)
                {
                    money = 1000;
                }
                GemEffect.SpawnGemEffects(level, money, pickup.Pos, pickup, false);
            }
            else
            {
                if (level.Difficulty == LevelDifficulty.hard)
                {
                    GemEffect.SpawnGemEffects(level, 250, pickup.Pos, pickup, false);
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
            level.PlaySound(GetPickupSoundID(pickup));
            level.Spawn(EffectID.starParticles, pickup.Pos, pickup);

        }
        private static Vector3 GetMoveTargetPosition(Entity entity)
        {
            var level = entity.Level;
            Vector3 slotPosition = level.GetScreenCenterPosition() + Vector2.down * (entity.GetSize().y * entity.RenderScale.y * 0.5f);
            return new Vector3(slotPosition.x, slotPosition.y - COLLECTED_Z - 15, COLLECTED_Z);
        }
        private NamespaceID GetPickupModelID(Entity entity)
        {
            var game = Global.Game;
            var level = entity.Level;
            if (level.IsRerun)
                return VanillaModelID.moneyChest;
            var stageMeta = game.GetStageMeta(level.StageID);
            var levelModel = stageMeta?.clearPickupModel;
            if (NamespaceID.IsValid(levelModel))
                return levelModel;
            return VanillaModelID.blueprintPickup;
        }
        private NamespaceID GetPickupSoundID(Entity entity)
        {
            if (entity.ModelID == VanillaModelID.mapPickup)
            {
                return SoundID.pick;
            }
            return SoundID.tap;
        }
        private const float COLLECTED_Z = 0;
    }
}