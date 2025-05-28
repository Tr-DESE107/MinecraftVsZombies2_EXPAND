using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Fragments;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.divineShield)]
    public class DivineShieldBuff : BuffDefinition
    {
        public DivineShieldBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_ROOT, VanillaModelKeys.divineShield, VanillaModelID.divineShield);
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback, priority: -100);
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            entity.PlaySound(VanillaSoundID.divineShield);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var model = buff.GetInsertedModel(VanillaModelKeys.divineShield);
            if (model == null)
                return;
            model.SetModelProperty("Size", entity.GetSize());
        }
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var input = param.input;
            if (NamespaceID.IsValid(input.ShieldTarget))
                return;
            var entity = input.Entity;
            var buff = entity.GetFirstBuff<DivineShieldBuff>();
            if (buff == null)
                return;

            buff.Remove();

            result.SetFinalValue(false);
            PlayBreakEffect(entity);
        }
        private void PostEntityDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (info.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            foreach (var buff in entity.GetBuffs<DivineShieldBuff>())
            {
                PlayBreakEffect(entity);
                buff.Remove();
            }
        }
        public static void PlayBreakEffect(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.crystalBreak, volume: 0.5f);
            var pos = entity.GetCenter();
            var fragment = entity.Level.Spawn(VanillaEffectID.fragment, pos, entity);
            Fragment.SetFragmentID(fragment, VanillaFragmentID.divineShield);
            Fragment.AddEmitSpeed(fragment, 50);
        }
        public const float HEALTH_SPEED = 1 / 6f;
        public const float MAX_PARASITE_HEALTH = 50;
        public static readonly VanillaBuffPropertyMeta<int> PROP_PARASITE_HEALTH = new VanillaBuffPropertyMeta<int>("ParasiteHealth");
        public const float DAMAGE = 100;
    }
}
