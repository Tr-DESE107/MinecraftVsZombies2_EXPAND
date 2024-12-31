using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.napstablook)]
    public class Napstablook : StateEnemy
    {
        public Napstablook(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
        }
        public override void PreTakeDamage(DamageInput input)
        {
            base.PreTakeDamage(input);
            var entity = input.Entity;
            if (entity == null)
                return;
            if (input.Effects.HasEffect(VanillaDamageEffects.WHACK))
            {
                Enrage(entity);
                SwordHeldItemDefinition.Paralyze(entity.Level);
            }
            input.Cancel();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
        }
        protected override int GetActionState(Entity enemy)
        {
            if (enemy.IsDead)
            {
                return VanillaEntityStates.DEAD;
            }
            else if (enemy.IsPreviewEnemy())
            {
                return VanillaEntityStates.IDLE;
            }
            else if (IsAngry(enemy))
            {
                return VanillaEntityStates.ATTACK;
            }
            else
            {
                return VanillaEntityStates.WALK;
            }
        }
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);
            if (IsAngry(enemy))
            {
                var ghostBuff = enemy.GetBuffs<GhostBuff>();
                foreach (var buff in ghostBuff)
                {
                    GhostBuff.Illuminate(buff);
                }
            }
        }
        public static void Enrage(Entity entity)
        {
            entity.AddBuff<NapstablookAngryBuff>();
        }
        public static bool IsAngry(Entity entity)
        {
            return entity.HasBuff<NapstablookAngryBuff>();
        }
    }
}
