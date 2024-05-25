using System.Linq;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public abstract class VanillaContraption : VanillaEntity, IEvokableContraption
    {
        public VanillaContraption(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.ATTACK_SPEED, 1f);
            SetProperty(EntityProperties.DAMAGE, 100f);
            SetProperty(EntityProperties.MAX_HEALTH, 300f);
            SetProperty(EntityProperties.FRICTION, 0.2f);
            SetProperty(EntityProperties.SHELL, ShellID.stone);
            SetProperty(EntityProperties.FALL_DAMAGE, 22.5f);
            SetProperty(EntityProps.DEATH_SOUND, SoundID.stone);
            SetProperty(ContraptionProps.FRAGMENT, GetID());
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetFaction(entity.Game.Option.LeftFaction);
            var fragment = CreateFragment(entity);
            var fragmentRef = new EntityReference(fragment);
            SetFragment(entity, fragmentRef);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            UpdateTakenGrids(entity);

            var fragment = GetOrCreateFragment(entity);
            Fragment.AddEmitSpeed(fragment, GetTickDamage(entity) * 0.1f);
            SetTickDamage(entity, 0);
        }
        public override void PostRemove(Entity entity)
        {
            base.PostRemove(entity);
            entity.ClearTakenGrids();
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            var fragment = GetOrCreateFragment(entity);
            Fragment.AddEmitSpeed(fragment, 50);

            entity.Game.PlaySound(entity.GetDeathSound(), entity.Pos);
            entity.Remove();
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                AddTickDamage(bodyResult.Entity, bodyResult.Amount);
            }
        }
        public virtual void Evoke(Contraption contraption)
        {
        }
        public static EntityReference GetFragment(Entity entity)
        {
            return entity.GetProperty<EntityReference>("Fragment");
        }
        public static void SetFragment(Entity entity, EntityReference value)
        {
            entity.SetProperty("Fragment", value);
        }
        public static float GetTickDamage(Entity entity)
        {
            return entity.GetProperty<float>("TickDamage");
        }
        public static void SetTickDamage(Entity entity, float value)
        {
            entity.SetProperty("TickDamage", value);
        }
        public static void AddTickDamage(Entity entity, float value)
        {
            SetTickDamage(entity, GetTickDamage(entity) + value);
        }
        private Entity CreateFragment(Entity entity)
        {
            var fragment = entity.Game.Spawn<Fragment>(entity.Pos, entity);
            fragment.SetParent(entity);
            return fragment;
        }
        private Entity GetOrCreateFragment(Entity entity)
        {
            var fragmentRef = GetFragment(entity);
            var fragment = fragmentRef?.GetEntity(entity.Game);
            if (fragment == null || !fragment.Exists())
            {
                fragment = CreateFragment(entity);
                fragmentRef = new EntityReference(fragment);
                SetFragment(entity, fragmentRef);
            }
            return fragment;
        }
        public override int Type => EntityTypes.PLANT;

    }
    public interface IEvokableContraption
    {
        void Evoke(Contraption contraption);
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