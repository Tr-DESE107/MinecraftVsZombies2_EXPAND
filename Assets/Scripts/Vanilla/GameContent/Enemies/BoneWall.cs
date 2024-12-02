using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.boneWall)]
    public class BoneWall : StateEnemy
    {
        public BoneWall(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
            entity.PlaySound(VanillaSoundID.boneWallBuild);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Die(new DamageInput(0, new DamageEffectList(), entity, new EntityReferenceChain(entity)));
                }
            }
        }
        public override void PostDeath(Entity entity, DamageInput info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            entity.Level.Spawn(VanillaEffectID.boneParticles, entity.GetCenter(), entity);
            entity.Remove();
        }
    }
}
