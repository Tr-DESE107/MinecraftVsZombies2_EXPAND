using System.Collections.Generic;
using System.Linq;
using PVZEngine.Base;
using PVZEngine.Buffs;
using PVZEngine.Level;
using Tools;

namespace PVZEngine.Auras
{
    public class AuraEffect
    {
        public AuraEffect(AuraEffectDefinition definition, int id, IAuraSource source)
        {
            ID = id;
            Definition = definition;
            updateTimer = new FrameTimer(Definition.UpdateInterval);
            Source = source;
        }
        public void UpdateAuraInterval()
        {
            updateTimer.Run();
            if (updateTimer.Expired)
            {
                updateTimer.Reset();
                UpdateAura();
            }
        }
        public void PostAdd()
        {
            UpdateAura();
            Definition.PostAdd(this);
        }
        public void PostRemove()
        {
            ClearBuffs();
            Definition.PostRemove(this);
        }
        public void UpdateAura()
        {
            var level = Source.GetLevel();
            targetsBuffer.Clear();
            Definition.GetAuraTargets(this, targetsBuffer);
            foreach (var target in targetsBuffer)
            {
                if (target == null)
                    continue;
                var buff = GetTargetBuff(target);
                if (buff == null)
                {
                    buff = AddTargetBuff(target);
                }
                Definition.UpdateTargetBuff(this, target, buff);
            }

            removeBuffBuffer.Clear();
            foreach (var pair in buffCaches)
            {
                var target = pair.Key;
                if (target == null)
                {
                    removeBuffBuffer.Add(target);
                    continue;
                }
                if (!target.Exists() || !targetsBuffer.Contains(target))
                {
                    var buff = pair.Value;
                    target.RemoveBuff(buff);
                    removeBuffBuffer.Add(target);
                }
            }
            for (int i = 0; i < removeBuffBuffer.Count; i++)
            {
                buffCaches.Remove(removeBuffBuffer[i]);
            }
        }
        public SerializableAuraEffect ToSerializable()
        {
            return new SerializableAuraEffect()
            {
                id = ID,
                buffs = buffCaches.Select(r => r.Key.GetBuffReference(r.Value)).ToArray(),
                updateTimer = updateTimer,
            };
        }
        public void LoadFromSerializable(LevelEngine level, SerializableAuraEffect serializable)
        {
            updateTimer = serializable.updateTimer;
            buffCaches.Clear();
            foreach (var seriBuff in serializable.buffs)
            {
                var entity = seriBuff.GetTarget(level);
                if (entity == null)
                    continue;
                var buff = seriBuff.GetBuff(level);
                if (buff == null)
                    continue;
                buffCaches.Add(entity, buff);
            }
        }
        private Buff AddTargetBuff(IBuffTarget target)
        {
            var buff = target.CreateBuff(Definition.BuffID);
            target.AddBuff(buff);
            buffCaches[target] = buff;
            return buff;
        }
        private Buff GetTargetBuff(IBuffTarget entity)
        {
            if (buffCaches.TryGetValue(entity, out var buff))
                return buff;
            return null;
        }
        private void ClearBuffs()
        {
            foreach (var pair in buffCaches)
            {
                var buff = pair.Value;
                if (buff == null)
                    continue;
                buff.Remove();
            }
            buffCaches.Clear();
        }
        public int ID { get; }
        public LevelEngine Level => Source.GetLevel();
        public AuraEffectDefinition Definition { get; }
        public IAuraSource Source { get; }
        private FrameTimer updateTimer;
        private Dictionary<IBuffTarget, Buff> buffCaches = new Dictionary<IBuffTarget, Buff>();
        private List<IBuffTarget> targetsBuffer = new List<IBuffTarget>();
        private ArrayBuffer<IBuffTarget> removeBuffBuffer = new ArrayBuffer<IBuffTarget>(1024);
    }
}
