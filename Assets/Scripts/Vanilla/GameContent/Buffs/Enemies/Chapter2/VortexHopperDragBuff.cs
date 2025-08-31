using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Enemy.vortexHopperDrag)]
    public class VortexHopperDragBuff : BuffDefinition
    {
        public VortexHopperDragBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var center = buff.GetProperty<Vector3>(PROP_CENTER);
            var angle = buff.GetProperty<float>(PROP_ANGLE);
            var radius = buff.GetProperty<float>(PROP_RADIUS);
            angle += 16;
            angle %= 360;
            buff.SetProperty(PROP_ANGLE, angle);

            Vector3 pos = entity.Position;
            float x = center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = center.z + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            var level = entity.Level;
            if (level.IsWaterAt(x, entity.Position.z))
            {
                pos.x = x;
            }
            if (level.IsWaterAt(entity.Position.x, z))
            {
                pos.z = z;
            }

            entity.Position = pos;

            radius = Mathf.Max(20, radius - 3);
            buff.SetProperty(PROP_RADIUS, radius);
        }
        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_CENTER = new VanillaBuffPropertyMeta<Vector3>("Center");
        public static readonly VanillaBuffPropertyMeta<float> PROP_RADIUS = new VanillaBuffPropertyMeta<float>("Radius");
        public static readonly VanillaBuffPropertyMeta<float> PROP_ANGLE = new VanillaBuffPropertyMeta<float>("Angle");
    }
}
