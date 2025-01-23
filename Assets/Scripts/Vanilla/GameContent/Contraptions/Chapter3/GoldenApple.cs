using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.goldenApple)]
    public class GoldenApple : ContraptionBehaviour
    {
        public GoldenApple(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENEMY_MELEE_ATTACK, PostEnemyMeleeAttackCallback);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationBool("Evoked", entity.IsEvoked());
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            entity.PlaySound(VanillaSoundID.sparkle);
        }

        private void PostEnemyMeleeAttackCallback(Entity enemy, Entity target)
        {
            if (!target.IsEntityOf(VanillaContraptionID.goldenApple))
                return;
            if (!target.IsHostile(enemy))
                return;
            if (target.IsAIFrozen())
                return;
            if (target.IsEvoked())
            {
                var mutant = target.Spawn(VanillaEnemyID.mutantZombie, enemy.Position);
                mutant.Charm(target.GetFaction());
                enemy.Spawn(VanillaEffectID.mindControlLines, enemy.GetCenter());
                enemy.Neutralize();
                enemy.Remove();
                enemy.PlaySound(VanillaSoundID.charmed);
                enemy.PlaySound(VanillaSoundID.odd);
            }
            else
            {
                enemy.Charm(target.GetFaction());
                enemy.Spawn(VanillaEffectID.mindControlLines, enemy.GetCenter());
                enemy.Neutralize();
                enemy.PlaySound(VanillaSoundID.charmed);
                enemy.PlaySound(VanillaSoundID.floop);
            }
            target.Remove();
        }
        private static readonly NamespaceID ID = VanillaContraptionID.goldenApple;
    }
}
