using System.Linq;
using MVZ2Logic.Level;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2Logic.Entities;
using PVZEngine.Base;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaContraption : VanillaEntity, IEvokableContraption
    {
        public VanillaContraption(string nsp, string name) : base(nsp, name)
        {
            SetProperty(VanillaContraptionProps.FRAGMENT_ID, GetID());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Level.Option.LeftFaction);

            entity.InitFragment();
        }
        public override sealed void Update(Entity entity)
        {
            base.Update(entity);
            if (!entity.IsAIFrozen())
            {
                UpdateAI(entity);
            }
            UpdateLogic(entity);
        }
        protected virtual void UpdateLogic(Entity entity)
        {
            UpdateTakenGrids(entity);
            entity.UpdateFragment();
        }
        protected virtual void UpdateAI(Entity entity)
        {
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.PostFragmentDeath(damageInfo);

            entity.PlaySound(entity.GetDeathSound());
            entity.Remove();
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                bodyResult.Entity.AddFragmentTickDamage(bodyResult.Amount);
            }
        }
        public virtual bool CanEvoke(Entity entity)
        {
            return !entity.IsEvoked();
        }
        public virtual void Evoke(Entity entity)
        {
            var bounds = entity.GetBounds();
            var pos = bounds.center;
            pos.z = entity.Position.z;
            entity.Level.Spawn(VanillaEffectID.evocationStar, pos, entity);
            OnEvoke(entity);
        }
        protected virtual void OnEvoke(Entity entity)
        {

        }
        public override int Type => EntityTypes.PLANT;

    }
    public class GradientInfo
    {
        public bool blend;
        public WeightedColor[] colors;
        public WeightedAlpha[] alphas;
        public Gradient ToGradient()
        {
            GradientColorKey[] colorKeys;
            GradientAlphaKey[] alphaKeys;
            if (colors == null || colors.Length <= 0)
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.magenta, 0.5f),
                    new GradientColorKey(Color.black, 1),
                };
            }
            else
            {
                colorKeys = new GradientColorKey[colors.Length];
                float totalWeight = colors.Sum(c => c.weight);
                float time = 0;
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    var weightedColor = colors[i];
                    var col = ColorUtility.TryParseHtmlString(weightedColor.hex, out Color color) ? color : Color.magenta;
                    var timeSpan = weightedColor.weight / totalWeight;
                    colorKeys[i] = new GradientColorKey(col, timeSpan + time);
                    time += timeSpan;
                }
            }
            if (alphas == null || alphas.Length <= 0)
            {
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0),
                };
            }
            else
            {
                alphaKeys = new GradientAlphaKey[alphas.Length];
                float totalWeight = alphas.Sum(c => c.weight);
                float time = 0;
                for (int i = 0; i < alphaKeys.Length; i++)
                {
                    var weightedAlpha = alphas[i];
                    var timeSpan = weightedAlpha.weight / totalWeight;
                    alphaKeys[i] = new GradientAlphaKey(weightedAlpha.alpha, timeSpan + time);
                    time += timeSpan;
                }
            }
            return new Gradient()
            {
                mode = blend ? GradientMode.Blend : GradientMode.Fixed,
                colorKeys = colorKeys,
                alphaKeys = alphaKeys,
            };
        }
    }
    public class WeightedColor
    {
        public string hex;
        public float weight;
        public WeightedColor(string hex, float weight)
        {
            this.hex = hex;
            this.weight = weight;
        }
    }
    public class WeightedAlpha
    {
        public float alpha;
        public float weight;
        public WeightedAlpha(float alpha, float weight)
        {
            this.alpha = alpha;
            this.weight = weight;
        }
    }
}