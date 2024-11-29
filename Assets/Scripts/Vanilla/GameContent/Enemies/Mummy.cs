using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.mummy)]
    [SpawnDefinition(2, previewCount: 3)]
    [EntitySeedDefinition(75, VanillaMod.spaceName, VanillaRechargeNames.none)]
    public class Mummy : MeleeEnemy
    {
        public Mummy(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public override void PostDeath(Entity entity, DamageInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            var gas = entity.Level.Spawn(VanillaEffectID.mummyGas, entity.Position, entity);
            gas.SetFaction(entity.GetFaction());
            entity.PlaySound(VanillaSoundID.poisonCast);
        }
    }
}
