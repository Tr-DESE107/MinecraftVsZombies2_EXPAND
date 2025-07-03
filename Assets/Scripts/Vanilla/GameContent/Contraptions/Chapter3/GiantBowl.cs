﻿using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.giantBowl)]
    public class GiantBowl : ContraptionBehaviour
    {
        public GiantBowl(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_PRODUCE_COLOR));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var productionTimer = new FrameTimer(PRODUCT_TIME);
            SetProductionTimer(entity, productionTimer);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            var timer = GetProductionTimer(entity);
            if (GetPointCount(entity) < MAX_STARSHARD_COUNT)
            {
                timer.Run(entity.GetProduceSpeed());
                if (timer.Expired)
                {
                    timer.Reset();
                    AddPointCount(entity, 1);
                    entity.PlaySound(VanillaSoundID.starshardUse);
                }
            }
            else
            {
                timer.Reset();
            }
            var pointsAngle = GetPointsAngle(entity);
            pointsAngle += ANGLE_SPEED;
            pointsAngle %= 360;
            SetPointsAngle(entity, pointsAngle);

            var pointsRadial = GetPointsRadial(entity);
            pointsRadial += RADIAL_SPEED;
            pointsRadial %= Mathf.PI * 2;
            SetPointsRadial(entity, pointsRadial);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            var produceColor = Color.clear;
            var timer = GetProductionTimer(entity);
            var thresold = PRODUCT_TIME * 0.5f;
            var progress = thresold - timer.Frame;
            if (progress > 0)
            {
                var x = Mathf.Pow(progress / (PRODUCT_TIME / 12.5f), 3);
                var alpha = (-Mathf.Cos(x) + 1) * 0.25f;
                produceColor = Color.white;
                produceColor.a = alpha;
            }
            SetProduceColor(entity, produceColor);

            entity.SetModelProperty("Count", GetPointCount(entity));
            entity.SetModelProperty("Angle", GetPointsAngle(entity));
            entity.SetModelProperty("Radial", (Mathf.Sin(GetPointsRadial(entity)) + 1) * 0.5f);
        }

        public override bool CanEvoke(Entity entity)
        {
            if (GetPointCount(entity) >= MAX_STARSHARD_COUNT)
                return false;
            return base.CanEvoke(entity);
        }

        protected override void OnEvoke(Entity contraption)
        {
            base.OnEvoke(contraption);
            AddPointCount(contraption, 1);
        }

        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            var points = GetPointCount(entity);
            for (int i = 0; i < points; i++)
            {
                entity.Spawn(VanillaPickupID.starshard, entity.Position);
            }
            SetPointCount(entity, 0);
        }

        public static Color GetProduceColor(Entity entity) => entity.GetProperty<Color>(PROP_PRODUCE_COLOR);
        public static void SetProduceColor(Entity entity, Color value) => entity.SetProperty(PROP_PRODUCE_COLOR, value);

        public static FrameTimer GetProductionTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, FIELD_PRODUCTION_TIMER);
        public static void SetProductionTimer(Entity entity, FrameTimer value) => entity.SetBehaviourField(ID, FIELD_PRODUCTION_TIMER, value);

        public static int GetPointCount(Entity entity) => entity.GetBehaviourField<int>(ID, FIELD_POINT_COUNT);
        public static void SetPointCount(Entity entity, int value) => entity.SetBehaviourField(ID, FIELD_POINT_COUNT, value);
        public static void AddPointCount(Entity entity, int value) => SetPointCount(entity, GetPointCount(entity) + value);

        public static float GetPointsAngle(Entity entity) => entity.GetBehaviourField<float>(ID, FIELD_POINTS_ANGLE);
        public static void SetPointsAngle(Entity entity, float value) => entity.SetBehaviourField(ID, FIELD_POINTS_ANGLE, value);

        public static float GetPointsRadial(Entity entity) => entity.GetBehaviourField<float>(ID, FIELD_POINTS_RADIAL);
        public static void SetPointsRadial(Entity entity, float value) => entity.SetBehaviourField(ID, FIELD_POINTS_RADIAL, value);


        private static readonly VanillaEntityPropertyMeta<FrameTimer> FIELD_PRODUCTION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ProductionTimer");
        private static readonly VanillaEntityPropertyMeta<int> FIELD_POINT_COUNT = new VanillaEntityPropertyMeta<int>("PointCount");
        private static readonly VanillaEntityPropertyMeta<float> FIELD_POINTS_ANGLE = new VanillaEntityPropertyMeta<float>("PointsAngle");
        private static readonly VanillaEntityPropertyMeta<float> FIELD_POINTS_RADIAL = new VanillaEntityPropertyMeta<float>("PointsRadial");
        private static readonly VanillaEntityPropertyMeta<Color> PROP_PRODUCE_COLOR = new VanillaEntityPropertyMeta<Color>("ProduceColor");

        public const int MAX_STARSHARD_COUNT = 5;
        public const int PRODUCT_TIME = 1800;
        public const float ANGLE_SPEED = 3f;
        public const float RADIAL_SPEED = 0.05f;
        private static readonly NamespaceID ID = VanillaContraptionID.giantBowl;
    }
}
