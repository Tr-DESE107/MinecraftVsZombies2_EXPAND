using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.mummy)]
    public class Mummy : MeleeEnemy
    {
        public Mummy(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelHealthStateByCount(2);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.SCALE, entity.GetScale());
            var gas = entity.Spawn(VanillaEffectID.mummyGas, entity.Position, param);
            entity.PlaySound(VanillaSoundID.poisonCast);
        }
    }
}
