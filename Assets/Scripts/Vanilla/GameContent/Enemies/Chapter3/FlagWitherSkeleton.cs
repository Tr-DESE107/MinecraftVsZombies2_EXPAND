using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Armors;
using MVZ2.Vanilla.Entities;
using MVZ2.GameContent.Buffs.Enemies;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.FlagWitherSkeleton)]
    public class FlagWitherSkeleton : WitherSkeleton
    {
        public FlagWitherSkeleton(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetAnimationBool("HasFlag", true);
            entity.EquipMainArmor(VanillaArmorID.ironHelmet);

            var speedBuff = entity.GetFirstBuff<RandomEnemySpeedBuff>();
            if (speedBuff != null)
            {
                RandomEnemySpeedBuff.SetSpeed(speedBuff, 2);
            }
        }
    }
}
