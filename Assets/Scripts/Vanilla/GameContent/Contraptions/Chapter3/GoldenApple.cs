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
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile |= EntityCollisionHelper.MASK_ENEMY;
        }

        private void PostEnemyMeleeAttackCallback(Entity enemy, Entity target)
        {
            if (!target.IsEntityOf(VanillaContraptionID.goldenApple))
                return;
            if (!target.IsHostile(enemy))
                return;
            if (target.IsAIFrozen())
                return;
            enemy.Charm(target.GetFaction());
            target.Remove();
            enemy.PlaySound(VanillaSoundID.floop);
            enemy.PlaySound(VanillaSoundID.charmed);
        }
        private static readonly NamespaceID ID = VanillaContraptionID.goldenApple;
    }
}
