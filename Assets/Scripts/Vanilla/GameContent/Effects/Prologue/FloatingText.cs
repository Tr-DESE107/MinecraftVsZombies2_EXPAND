using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.floatingText)]
    public class FloatingText : EffectBehaviour
    {
        public FloatingText(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var tint = entity.GetTint();
            tint.a = Mathf.Clamp01(entity.Timeout / 15f);
            entity.SetTint(tint);
            entity.SetModelProperty("Color", tint);
            entity.SetModelProperty("Text", GetText(entity));
        }
        public static string GetText(Entity entity) => entity.GetBehaviourField<string>(ID, PROP_TEXT);
        public static void SetText(Entity entity, string value) => entity.SetBehaviourField(ID, PROP_TEXT, value);

        public static readonly VanillaEntityPropertyMeta<string> PROP_TEXT = new VanillaEntityPropertyMeta<string>("Text");
        public static readonly NamespaceID ID = VanillaEffectID.floatingText;
    }
}