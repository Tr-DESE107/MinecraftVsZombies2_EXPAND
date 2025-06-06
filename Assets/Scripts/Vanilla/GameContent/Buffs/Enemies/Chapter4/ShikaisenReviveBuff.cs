using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.shikaisenRevive)]
    public class ShikaisenReviveBuff : BuffDefinition
    {
        public ShikaisenReviveBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEnemyProps.ASSUME_ALIVE, true));
            AddTrigger(VanillaLevelCallbacks.PRE_ENEMY_FAINT, PreEnemyFaintCallback);
        }
        private void PreEnemyFaintCallback(EntityCallbackParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            Buff buff = null;
            Entity source = null;
            foreach (var b in entity.GetBuffs(this))
            {
                var sourceID = GetSource(b);
                if (sourceID != null)
                {
                    var src = sourceID.GetEntity(level);
                    if (src.ExistsAndAlive())
                    {
                        buff = b;
                        source = src;
                        break;
                    }
                }
            }
            if (buff == null || !source.ExistsAndAlive())
                return;
            entity.Revive();
            var costHealth = Mathf.Min(source.Health, entity.GetMaxHealth());
            entity.HealEffects(costHealth, source);
            source.TakeDamage(costHealth, new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE), source);
            if (source.Health <= 0)
            {
                source.Die();
            }
            result.SetFinalValue(false);

            entity.PlaySound(VanillaSoundID.revived);
            buff.Remove();
        }
        public static void SetSource(Buff buff, EntityID id) => buff.SetProperty<EntityID>(PROP_SOURCE, id);
        public static EntityID GetSource(Buff buff) => buff.GetProperty<EntityID>(PROP_SOURCE);
        public static readonly VanillaBuffPropertyMeta<EntityID> PROP_SOURCE = new VanillaBuffPropertyMeta<EntityID>("source");
    }
}
