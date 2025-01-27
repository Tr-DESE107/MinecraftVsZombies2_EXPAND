using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.dullahanHead)]
    public class DullahanHead : MeleeEnemy
    {
        public DullahanHead(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 20);
        }
    }
}
