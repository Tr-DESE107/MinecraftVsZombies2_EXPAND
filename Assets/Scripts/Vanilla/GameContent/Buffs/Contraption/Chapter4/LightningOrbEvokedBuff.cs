using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Base;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.lightningOrbEvoked)]
    public class LightningOrbEvokedBuff : BuffDefinition
    {
        public LightningOrbEvokedBuff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
            thunderDetector = new LaneDetector(64, 64);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMER, new FrameTimer(MAX_TIMEOUT));
            SetSoundTimer(buff, new FrameTimer(0));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timer = buff.GetProperty<FrameTimer>(PROP_TIMER);
            var entity = buff.GetEntity();
            if (timer == null || timer.Expired)
            {
                var damage = GetTakenDamage(buff);
                damage = Mathf.Min(damage, MAX_DAMAGE);
                if (damage > 0)
                {
                    var level = buff.Level;
                    level.Thunder();
                    if (entity != null)
                    {
                        thunderBuffer.Clear();
                        thunderDetector.DetectMultiple(entity, thunderBuffer);
                        for (int i = 0; i < thunderBuffer.Count; i++)
                        {
                            var collider = thunderBuffer[i];
                            collider.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.LIGHTNING, VanillaDamageEffects.MUTE), entity);
                        }
                        var centerPos = entity.GetCenter();
                        for (int i = 0; i < 2; i++)
                        {
                            var thunder = level.Spawn(VanillaEffectID.thunderBoltHorizontal, centerPos, entity);
                            var color = new Color(0, 0.733333f, 1);
                            thunder.SetHSVToColor(color);
                            thunder.SetLightColor(color);
                            var scale = new Vector3(i * 2 - 1, 1, 1);
                            thunder.SetDisplayScale(scale);
                            thunder.SetScale(scale);
                        }
                        entity.PlaySound(VanillaSoundID.thunder);
                        entity.PlaySound(VanillaSoundID.tridentThunder);
                        entity.PlaySound(VanillaSoundID.teslaAttack);
                    }
                }
                buff.Remove();
            }
            else
            {
                timer.Run();
                var soundTimer = GetSoundTimer(buff);
                if (soundTimer != null)
                {
                    if (soundTimer.Expired)
                    {
                        soundTimer.ResetTime(15);
                    }
                    if (entity != null)
                    {
                        var pitch = 1 + (1 - timer.Frame) / (float)MAX_TIMEOUT;
                        entity.PlaySound(VanillaSoundID.energyShield, pitch);
                    }
                }
            }
        }
        private void PreEntityTakeDamageCallback(DamageInput damage)
        {
            var entity = damage.Entity;
            foreach (var buff in entity.GetBuffs<LightningOrbEvokedBuff>())
            {
                AddTakenDamage(buff, damage.Amount);
                damage.Cancel();
            }
        }
        public static FrameTimer GetSoundTimer(Buff buff) => buff.GetProperty<FrameTimer>(PROP_SOUND_TIMER);
        public static void SetSoundTimer(Buff buff, FrameTimer value) => buff.SetProperty(PROP_SOUND_TIMER, value);
        public static float GetTakenDamage(Buff buff) => buff.GetProperty<float>(PROP_TAKEN_DAMAGE);
        public static void SetTakenDamage(Buff buff, float value) => buff.SetProperty(PROP_TAKEN_DAMAGE, value);
        public static void AddTakenDamage(Buff buff, float value) => SetTakenDamage(buff, GetTakenDamage(buff) + value);
        public const int MAX_TIMEOUT = 90;
        public const float MAX_DAMAGE = 1800;
        public static readonly VanillaBuffPropertyMeta PROP_TIMER = new VanillaBuffPropertyMeta("timer");
        public static readonly VanillaBuffPropertyMeta PROP_TAKEN_DAMAGE = new VanillaBuffPropertyMeta("takenDamage");
        public static readonly VanillaBuffPropertyMeta PROP_SOUND_TIMER = new VanillaBuffPropertyMeta("soundTimer");
        private Detector thunderDetector;
        private ArrayBuffer<IEntityCollider> thunderBuffer = new ArrayBuffer<IEntityCollider>(1024);
    }
}
