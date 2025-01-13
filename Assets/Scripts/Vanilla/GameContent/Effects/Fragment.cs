using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.fragment)]
    public class Fragment : EffectBehaviour
    {

        #region 公有方法
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
            return entity.GetBehaviourField<float>(ID, "EmitSpeed");
        }
        public static void SetEmitSpeed(Entity entity, float value)
        {
            entity.SetBehaviourField(ID, "EmitSpeed", value);
        }
        public static void AddEmitSpeed(Entity entity, float value)
        {
            SetEmitSpeed(entity, GetEmitSpeed(entity) + value);
        }
        public static void UpdateFragmentID(Entity entity)
        {
            entity.SetModelProperty("FragmentID", entity.Parent?.GetFragmentID());
        }
        #endregion
        private static readonly NamespaceID ID = VanillaEffectID.fragment;
    }
}