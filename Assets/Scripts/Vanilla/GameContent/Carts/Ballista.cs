using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Carts
{
    [EntityBehaviourDefinition(VanillaCartNames.ballista)]
    public class Ballista : CartBehaviour
    {
        public Ballista(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            bool loaded = !entity.IsCartTriggered();
            float blend = GetStringBlend(entity);
            float targetBlend = loaded ? 1 : 0;
            blend = blend * 0.8f + targetBlend * 0.2f;
            SetStringBlend(entity, blend);

            entity.SetAnimationBool("Loaded", loaded);
            entity.SetAnimationFloat("StringBlend", blend);
        }
        public override void PostTrigger(Entity entity)
        {
            base.PostTrigger(entity);
            entity.ShootProjectile();
        }
        public static float GetStringBlend(Entity entity)
        {
            return entity.GetBehaviourField<float>(PROP_STRING_BLEND);
        }
        public static void SetStringBlend(Entity entity, float value)
        {
            entity.SetBehaviourField(PROP_STRING_BLEND, value);
        }
        public static readonly VanillaEntityPropertyMeta<float> PROP_STRING_BLEND = new VanillaEntityPropertyMeta<float>("StringBlend");
    }
}