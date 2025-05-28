using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.fragment)]
    public class Fragment : EffectBehaviour
    {

        #region ���з���
        public Fragment(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var parent = entity.Parent;
            if (parent != null && parent.Exists())
            {
                entity.Timeout = 30;
                entity.Position = parent.Position;
                UpdateFragmentID(entity);
            }
            entity.SetModelProperty("EmitSpeed", GetEmitSpeed(entity));
            SetEmitSpeed(entity, 0);
        }
        public static float GetEmitSpeed(Entity entity)
        {
            return entity.GetBehaviourField<float>(ID, PROP_EMIT_SPEED);
        }
        public static void SetEmitSpeed(Entity entity, float value)
        {
            entity.SetBehaviourField(ID, PROP_EMIT_SPEED, value);
        }
        public static void AddEmitSpeed(Entity entity, float value)
        {
            SetEmitSpeed(entity, GetEmitSpeed(entity) + value);
        }
        public static void SetFragmentID(Entity entity, NamespaceID value)
        {
            entity.SetModelProperty("FragmentID", value);
        }
        public static void UpdateFragmentID(Entity entity)
        {
            var parent = entity?.Parent;
            SetFragmentID(entity, parent?.GetFragmentID() ?? parent?.GetDefinitionID());
        }
        #endregion
        private static readonly NamespaceID ID = VanillaEffectID.fragment;
        public static readonly VanillaEntityPropertyMeta<float> PROP_EMIT_SPEED = new VanillaEntityPropertyMeta<float>("EmitSpeed");
    }
}