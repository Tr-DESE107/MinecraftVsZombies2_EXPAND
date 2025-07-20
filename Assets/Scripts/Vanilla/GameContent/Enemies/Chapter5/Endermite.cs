using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Enemies;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.Endermite)]
    public class Endermite : MeleeEnemy
    {
        public Endermite(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            
        }

    }
}
