using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Difficulties;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareaperTimer)]
    public class NightmareaperTimer : EffectBehaviour
    {
        public NightmareaperTimer(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var timeout = 2700 + entity.Level.GetBossAILevel() * 900;
            SetTimeout(entity, timeout);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            UpdateTimer(entity);
        }
        private void UpdateTimer(Entity entity)
        {
            var timeout = GetTimeout(entity);
            if (timeout > 0)
            {
                timeout--;
                if (timeout <= 0)
                {
                    EnrageReapers(entity);
                }
                SetTimeout(entity, timeout);
            }
            Color tint = Color.Lerp(Color.red, Color.white, timeout / 900f);
            entity.SetModelProperty("Color", tint);
            entity.SetModelProperty("Timeout", timeout);
        }
        private void EnrageReapers(Entity entity)
        {
            var level = entity.Level;
            level.StopMusic();
            foreach (Entity nightmareaper in level.FindEntities(VanillaBossID.nightmareaper))
            {
                Nightmareaper.Enrage(nightmareaper);
                nightmareaper.AddBuff<NightmareaperEnragedBuff>();
            }
        }
        public static int GetTimeout(Entity entity) => entity.GetBehaviourField<int>(ID, PROP_TIMEOUT);
        public static void SetTimeout(Entity entity, int value) => entity.SetBehaviourField(ID, PROP_TIMEOUT, value);
        public static readonly VanillaEntityPropertyMeta PROP_TIMEOUT = new VanillaEntityPropertyMeta("Timeout");
        public static readonly NamespaceID ID = VanillaEffectID.nightmareaperTimer;
    }
}