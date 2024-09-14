using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.LevelManagement;

namespace MVZ2.GameContent.Effects
{
    [Definition(EffectNames.fragment)]
    public class Fragment : VanillaEffect
    {

        #region 公有方法
        public Fragment(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            entity.Timeout = 60;
        }
        public override void Update(Entity entity)
        {
            var parent = entity.Parent;
            if (parent == null || !parent.Exists())
            {
                entity.Timeout--;
            }
            else
            {
                entity.Timeout = 30;
                entity.Pos = parent.Pos;
                entity.SetModelProperty("FragmentID", parent.GetProperty<NamespaceID>(ContraptionProps.FRAGMENT));
            }
            entity.SetAnimationFloat("EmitSpeed", GetEmitSpeed(entity));
            SetEmitSpeed(entity, 0);
            if (entity.Timeout <= 0)
            {
                entity.Remove();
            }

        }
        public static float GetEmitSpeed(Entity entity)
        {
            return entity.GetProperty<float>("EmitSpeed");
        }
        public static void SetEmitSpeed(Entity entity, float value)
        {
            entity.SetProperty("EmitSpeed", value);
        }
        public static void AddEmitSpeed(Entity entity, float value)
        {
            SetEmitSpeed(entity, GetEmitSpeed(entity) + value);
        }
        #endregion
    }
}