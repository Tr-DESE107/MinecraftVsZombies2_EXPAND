using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.infectenser)]
    public class Infectenser : DispenserFamily
    {
        public Infectenser(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            InitShootTimer(entity);
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (!entity.IsEvoked())
            {
                ShootTick(entity);
                return;
            }
        }
        public override Entity Shoot(Entity entity)
        {
            var projectile = base.Shoot(entity);
            projectile.Timeout = Mathf.CeilToInt(entity.GetRange() / entity.GetShotVelocity().magnitude);
            return projectile;
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            bool playSound = false;
            foreach (Entity target in entity.Level.FindEntities(e => e.HasBuff<ParabotBuff>()))
            {
                var buffs = target.GetBuffs<ParabotBuff>();
                foreach (var buff in buffs)
                {
                    if (entity.IsFriendly(buff.GetProperty<int>(ParabotBuff.PROP_FACTION)))
                    {
                        buff.SetProperty(ParabotBuff.PROP_EXPLODE_TIME, MAX_EXPLOSION_TIMEOUT);
                        playSound = true;
                    }
                }
            }
            if (playSound)
            {
                entity.PlaySound(VanillaSoundID.parabotTick);
            }
        }
        public const int MAX_EXPLOSION_TIMEOUT = 24;
    }
}
