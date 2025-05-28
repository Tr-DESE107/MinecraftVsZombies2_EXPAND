﻿using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.wickedHermitZombie)]
    public class WickedHermitZombie : MeleeEnemy
    {
        public WickedHermitZombie(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipMainArmor(VanillaArmorID.wickedHermitHat);

            if (!entity.IsPreviewEnemy() && !entity.Level.IsIZombie())
            {
                var talisman = entity.SpawnWithParams(VanillaEnemyID.talismanZombie, entity.Position);
                SetTalismanZombie(entity, new EntityID(talisman));
            }
        }
        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);
            if (!IsWarpped(enemy) && !enemy.IsDead)
            {
                var talismanID = GetTalismanZombie(enemy);
                var talisman = talismanID?.GetEntity(enemy.Level);
                if (!talisman.ExistsAndAlive() || !talisman.IsFriendly(enemy))
                {
                    SetTalismanZombie(enemy, null);
                    enemy.AddBuff<WickedHermitWarpBuff>();
                    SetWarpped(enemy, true);
                    enemy.PlaySound(VanillaSoundID.gapWarp);
                }
            }
        }
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);
            if (state == VanillaEntityStates.WALK)
            {
                if (!IsWarpped(enemy))
                {
                    var talismanID = GetTalismanZombie(enemy);
                    var talisman = talismanID?.GetEntity(enemy.Level);
                    if (talisman != null)
                    {
                        bool tooClose;
                        if (enemy.IsFacingLeft())
                        {
                            tooClose = talisman.Position.x > enemy.Position.x - TALISMAN_DISTANCE;
                        }
                        else
                        {
                            tooClose = talisman.Position.x < enemy.Position.x + TALISMAN_DISTANCE;
                        }
                        if (tooClose)
                        {
                            state = VanillaEntityStates.IDLE;
                        }
                    }
                }
            }
            return state;
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public static void SetWarpped(Entity entity, bool value)
        {
            entity.SetBehaviourField(PROP_WARPPED, value);
        }
        public static bool IsWarpped(Entity entity)
        {
            return entity.GetBehaviourField<bool>(PROP_WARPPED);
        }
        public static void SetTalismanZombie(Entity entity, EntityID value)
        {
            entity.SetBehaviourField(PROP_TALISMAN_ZOMBIE, value);
        }
        public static EntityID GetTalismanZombie(Entity entity)
        {
            return entity.GetBehaviourField<EntityID>(PROP_TALISMAN_ZOMBIE);
        }
        public const int MOVE_INTERVAL = 30;
        public const float TALISMAN_DISTANCE = 80;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_WARPPED = new VanillaEntityPropertyMeta<bool>("warpped");
        public static readonly VanillaEntityPropertyMeta<EntityID> PROP_TALISMAN_ZOMBIE = new VanillaEntityPropertyMeta<EntityID>("MoveTimer");
    }
}
