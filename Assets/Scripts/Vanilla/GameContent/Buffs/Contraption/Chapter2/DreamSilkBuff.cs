using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Models;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using Tools.Mathematics;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.dreamSilk)]
    public class DreamSilkBuff : BuffDefinition
    {
        public DreamSilkBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.dreamAlarm, VanillaModelID.dreamAlarm);
            AddModifier(new BooleanModifier(VanillaEntityProps.AI_FROZEN, true));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_DISPLAY_SCALE));
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback, filter: EntityTypes.PLANT);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
            buff.SetProperty(PROP_DISPLAY_SCALE, Vector3.one);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var contraption = buff.GetEntity();
            if (contraption == null)
                return;
            // Dream.
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            timer.Run();
            if (timer.PassedFrame(RING_DURATION))
            {
                contraption.PlaySound(VanillaSoundID.dreamAlarm);
                contraption.PlaySound(VanillaSoundID.wakeup);
            }
            if (timer.Frame <= RING_DURATION)
            {
                var scale = Vector3.one;
                var t = 1 - (timer.Frame / (float)RING_DURATION);
                scale.y = GetDisplayScaleY(t * 2);
                buff.SetProperty(PROP_DISPLAY_SCALE, scale);
            }
            if (timer.Expired)
            {
                var level = buff.Level;
                var position = contraption.GetCenter();
                level.Spawn(VanillaPickupID.starshard, position, contraption);
                var cluster = level.Spawn(VanillaEffectID.smokeCluster, position, contraption);
                cluster.SetTint(new Color(1, 0.8f, 1, 1));
                buff.Remove();
            }

            var model = buff.GetInsertedModel(VanillaModelKeys.dreamAlarm);
            if (model != null)
            {
                model.SetAnimationBool("Awake", timer.Frame <= RING_DURATION);
                model.SetAnimationFloat("Time", (timer.Frame - RING_DURATION) / (float)(MAX_TIMEOUT - RING_DURATION));
            }
        }
        private static float GetDisplayScaleY(float percentage)
        {
            if (percentage <= 0.25f)
            {
                return 1 - 0.2f * MathTool.EaseInAndOut(percentage / 0.25f);
            }
            else if (percentage <= 0.75f)
            {
                return 0.8f + 0.45f * MathTool.EaseInAndOut((percentage - 0.25f) / 0.5f);
            }
            else if (percentage <= 1)
            {
                return 1.25f - 0.25f * MathTool.EaseInAndOut((percentage - 0.75f) / 0.25f);
            }
            return 1;
        }
        private void PostEntityDeathCallback(Entity entity, DeathInfo info)
        {
            entity.RemoveBuffs<DreamSilkBuff>();
        }
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("Timer");
        public static readonly VanillaBuffPropertyMeta PROP_DISPLAY_SCALE = new VanillaBuffPropertyMeta("DisplayScale");
        public const int MAX_TIMEOUT = 900;
        private const int RING_DURATION = 36;
    }
}
