using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class AchievementsImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostContraptionDeathCallback, filter: EntityTypes.PLANT);
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
            mod.AddTrigger(VanillaLevelCallbacks.POST_OBSIDIAN_FIRST_AID, PostAnvilObsidianFirstAidCallback, filter: VanillaContraptionID.anvil);
        }
        private void PostContraptionDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (entity.IsHostileEntity() && !entity.Level.IsIZombie())
            {
                var level = entity.Level;
                var killedByFriendlyEnemy = info.Source.IsEntitySourceOf(level, (s, def) => def.Type == EntityTypes.ENEMY && level.IsFriendlyFaction(s.Faction));
                if (killedByFriendlyEnemy)
                {
                    Global.Game.Unlock(VanillaUnlockID.mesmerisedMatchup);
                }
            }
        }
        private void PostEnemyDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (entity.IsEntityOf(VanillaEnemyID.skeleton) && info.Effects.HasEffect(VanillaDamageEffects.FALL_DAMAGE) && !entity.Level.IsIZombie())
            {
                Global.Game.Unlock(VanillaUnlockID.bonebreaker);
            }

            if (entity.IsFriendlyEntity() && !entity.Level.IsIZombie())
            {
                var level = entity.Level;
                var killedByHostileContraption = info.Source.IsEntitySourceOf(level, (s, def) => def.Type == EntityTypes.PLANT && level.IsHostileFaction(s.Faction));
                if (killedByHostileContraption)
                {
                    Global.Game.Unlock(VanillaUnlockID.mesmerisedMatchup);
                }
            }
        }
        private void PostAnvilObsidianFirstAidCallback(EntityCallbackParams param, CallbackResult result)
        {
            Global.Game.Unlock(VanillaUnlockID.reforged);
        }
    }
}
