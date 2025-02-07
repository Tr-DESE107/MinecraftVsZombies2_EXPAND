using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.crushingWalls)]
    public class CrushingWalls : EffectBehaviour
    {

        #region 公有方法
        public CrushingWalls(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaCallbacks.POST_POINTER_ACTION, PostPointerActionCallback, filter: PointerPhase.Press);
        }
        #endregion

        public void PostPointerActionCallback(int type, int index, Vector2 screenPosition, PointerPhase phase)
        {
            if (!Global.Game.IsInLevel())
                return;
            var level = Global.Game.GetLevel();
            if (!level.IsGameRunning())
                return;
            foreach (var wall in level.FindEntities(VanillaEffectID.crushingWalls))
            {
                if (wall.State == VanillaEntityStates.CRUSHING_WALLS_IDLE)
                {
                    var progress = GetProgress(wall);
                    SetProgress(wall, progress - 0.01f);
                }
            }
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetSortingLayer(SortingLayers.frontUI);
            entity.SetSortingOrder(-9999);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            UpdateState(entity);
            UpdateShake(entity);
        }
        public static void Shake(Entity wall, float amplitudeStart, float amplitudeEnd, int time)
        {
            SetShake(wall, new ShakeInt(amplitudeStart, amplitudeEnd, time));
        }
        public static void Enrage(Entity wall)
        {
            wall.State = VanillaEntityStates.CRUSHING_WALLS_ENRAGED;
        }
        public static void Close(Entity wall)
        {
            wall.State = VanillaEntityStates.CRUSHING_WALLS_CLOSED;
        }
        private void UpdateState(Entity entity)
        {
            var progress = GetProgress(entity);
            switch (entity.State)
            {
                case VanillaEntityStates.CRUSHING_WALLS_IDLE:
                    {
                        var speed = 4;
                        var difficulty = entity.Level.Difficulty;
                        if (difficulty == VanillaDifficulties.easy)
                        {
                            speed = 3;
                        }
                        else if (difficulty == VanillaDifficulties.hard)
                        {
                            speed = 5;
                        }

                        progress += speed * 0.01f / 30f;
                        progress = Mathf.Clamp01(progress);
                        entity.SetModelProperty("Progress", progress);
                    }
                    break;
                case VanillaEntityStates.CRUSHING_WALLS_ENRAGED:
                    {
                        progress *= 0.8f;
                        entity.SetModelProperty("Progress", progress);
                    }
                    break;
                case VanillaEntityStates.CRUSHING_WALLS_CLOSED:
                    {
                        progress += 1 / 15f;
                        var realProgress = MathTool.EaseIn(progress);
                        if (progress >= 1)
                        {
                            entity.Level.ShakeScreen(10, 0, 15);
                            entity.PlaySound(VanillaSoundID.smash);
                        }
                        entity.SetModelProperty("Progress", realProgress);
                    }
                    break;
                case VanillaEntityStates.CRUSHING_WALLS_STOPPED:
                    {
                        progress *= 0.6f;
                        if (progress <= 0.02f)
                        {
                            entity.Remove();
                        }
                        entity.SetModelProperty("Progress", progress);
                    }
                    break;

            }
            SetProgress(entity, progress);


            if (progress >= 1)
            {
                entity.Level.GameOver(GameOverTypes.NO_ENEMY, entity, VanillaStrings.DEATH_MESSAGE_CRUSHING_WALLS);
            }
        }
        private void UpdateShake(Entity entity)
        {
            var shake = GetShake(entity);
            Vector3 shakeValue = Vector3.zero;
            if (shake != null)
            {
                shakeValue = shake.GetShake3D();
                shake.Run();
                if (shake.Expired)
                {
                    SetShake(entity, null);
                }
            }
            switch (entity.State)
            {
                case VanillaEntityStates.CRUSHING_WALLS_IDLE:
                    shakeValue += new Vector3(Random.Range(3, 3), Random.Range(3, 3), 0);
                    break;
            }
            entity.SetModelProperty("Shake", shakeValue);
        }
        public static float GetProgress(Entity entity) => entity.GetBehaviourField<float>(ID, PROP_PROGRESS);
        public static void SetProgress(Entity entity, float value) => entity.SetBehaviourField(ID, PROP_PROGRESS, value);
        public static ShakeInt GetShake(Entity entity) => entity.GetBehaviourField<ShakeInt>(ID, PROP_SHAKE);
        public static void SetShake(Entity entity, ShakeInt value) => entity.SetBehaviourField(ID, PROP_SHAKE, value);
        public static readonly VanillaEntityPropertyMeta PROP_PROGRESS = new VanillaEntityPropertyMeta("Progress");
        public static readonly VanillaEntityPropertyMeta PROP_SHAKE = new VanillaEntityPropertyMeta("Shake");
        public static readonly NamespaceID ID = VanillaEffectID.crushingWalls;
    }
}