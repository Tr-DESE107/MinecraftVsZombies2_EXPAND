using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.gunpowder)]
    public class Gunpowder : PickupBehaviour
    {
        public Gunpowder(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            var timeout = pickup.Timeout;
            bool collected = pickup.IsCollected();
            float r = 0;
            if (!collected && timeout <= TWINKLE_TIMEOUT)
            {
                var percentage = (1 - timeout / (float)TWINKLE_TIMEOUT);
                var x = Mathf.Pow(percentage * TWINKLE_SPEED, 3);
                r = (-Mathf.Cos(x) + 1) * 0.5f;
            }
            var offset = new Color(1, 0, 0, r);
            SetColorOffset(pickup, offset);

            if (!collected && timeout <= 1 && pickup.Exists())
            {
                var damage = pickup.GetDamage() * pickup.Level.GetGunpowderDamageMultiplier();
                var range = pickup.GetRange();
                var effects = new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE);
                pickup.ExplodeAgainstFriendly(pickup.GetCenter(), range, pickup.GetFaction(), pickup.GetDamage() * damage, effects);

                var explosionParam = pickup.GetSpawnParams();
                explosionParam.SetProperty(EngineEntityProps.SIZE, Vector3.one * (range * 2));
                pickup.Spawn(VanillaEffectID.explosion, pickup.GetCenter(), explosionParam);

                pickup.PlaySound(VanillaSoundID.explosion);

                pickup.Remove();
            }
        }
        public static Color GetColorOffset(Entity entity) => entity.GetBehaviourField<Color>(PROP_COLOR_OFFSET);
        public static void SetColorOffset(Entity entity, Color value) => entity.SetBehaviourField(PROP_COLOR_OFFSET, value);
        public const int TWINKLE_TIMEOUT = 150;
        public const float TWINKLE_SPEED = 5f;
        private static readonly VanillaEntityPropertyMeta<Color> PROP_COLOR_OFFSET = new VanillaEntityPropertyMeta<Color>("color_offset");
    }
}