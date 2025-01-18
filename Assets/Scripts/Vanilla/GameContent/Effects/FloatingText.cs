using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.floatingText)]
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

        public const string PROP_TEXT = "Text";
        public static readonly NamespaceID ID = VanillaEffectID.floatingText;
    }
}