using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.terrorParasitized)]
    public class TerrorParasitizedBuff : BuffDefinition
    {
        public TerrorParasitizedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.terrorParasitized, VanillaModelID.terrorParasitized);
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_PARASITE_HEALTH, -50f);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var health = buff.GetProperty<float>(PROP_PARASITE_HEALTH);
            health += HEALTH_SPEED;
            buff.SetProperty(PROP_PARASITE_HEALTH, health);

            var iconModel = buff.GetInsertedModel(VanillaModelKeys.terrorParasitized);
            if (iconModel != null)
            {
                iconModel.SetAnimationBool("Awake", health > 0);
            }
            if (health >= MAX_PARASITE_HEALTH)
            {
                SpawnParasites(entity, health);
                buff.Remove();
            }
        }
        public static void SpawnParasites(Entity host, float health)
        {
            var level = host.Level;
            host.TakeDamage(DAMAGE, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.SELF_DAMAGE, VanillaDamageEffects.MUTE), host);
            int count = 3;
            if (level.Difficulty == VanillaDifficulties.easy)
            {
                count = 2;
            }
            else if (level.Difficulty == VanillaDifficulties.hard)
            {
                count = 4;
            }
            for (int i = 0; i < count; i++)
            {
                var parasite = level.Spawn(VanillaEnemyID.parasiteTerror, host.GetCenter(), host);
                parasite.Health = health;
                parasite.SetFactionAndDirection(host.GetFaction());
            }
            host.PlaySound(VanillaSoundID.bloody);
            host.EmitBlood();
        }
        private void PostEntityDeathCallback(Entity entity, DeathInfo info)
        {
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            foreach (var buff in entity.GetBuffs<TerrorParasitizedBuff>())
            {
                var health = buff.GetProperty<float>(PROP_PARASITE_HEALTH);
                if (health > 0)
                {
                    SpawnParasites(entity, health);
                }
                buff.Remove();
            }
        }
        public const float HEALTH_SPEED = 1 / 6f;
        public const float MAX_PARASITE_HEALTH = 50;
        public static readonly VanillaBuffPropertyMeta PROP_PARASITE_HEALTH = new VanillaBuffPropertyMeta("ParasiteHealth");
        public const float DAMAGE = 100;
    }
}
