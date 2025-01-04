using System.Collections.Generic;
using System.Linq;
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
            for (int i = buffCaches.Count - 1; i >= 0; i--)
            {
                var reference = buffCaches[i];
                var target = reference.GetTarget(level);
                if (target == null)
                {
                    buffCaches.RemoveAt(i);
                    continue;
                }
                if (!target.Exists() || !targetsBuffer.Contains(target))
                {
                    var buff = reference.GetBuff(level);
                    target.RemoveBuff(buff);
                    buffCaches.RemoveAt(i);
                }
            }
        }
        public SerializableAuraEffect ToSerializable()
        {
            return new SerializableAuraEffect()
            {
                id = ID,
                buffs = buffCaches.Select(r => r.reference).ToArray(),
                updateTimer = updateTimer,
            };
        }
        public void LoadFromSerializable(LevelEngine level, SerializableAuraEffect serializable)
        {
            updateTimer = serializable.updateTimer;
            buffCaches.Clear();
            buffCaches.AddRange(serializable.buffs.Select(b => new BuffCache(b)));
        }
        private Buff AddTargetBuff(IBuffTarget target)
        {
            var buff = target.CreateBuff(Definition.BuffID);
            target.AddBuff(buff);
            buffCaches.Add(new BuffCache(target, buff));
            return buff;
        }
        private Buff GetTargetBuff(IBuffTarget entity)
        {
            var reference = buffCaches.Find(r => r.GetTarget(Level) == entity);
            if (reference == null)
                return null;
            return reference.GetBuff(Level);
        }
        private void ClearBuffs()
        {
            foreach (var reference in buffCaches)
            {
                var buff = reference.GetBuff(Level);
                buff.Remove();
            }
            buffCaches.Clear();
        }
        public int ID { get; }
        public LevelEngine Level => Source.GetLevel();
        public AuraEffectDefinition Definition { get; }
        public IAuraSource Source { get; }
        private FrameTimer updateTimer;
        private List<BuffCache> buffCaches = new List<BuffCache>();
        private List<IBuffTarget> targetsBuffer = new List<IBuffTarget>();
        private class BuffCache
        {
            public BuffCache(IBuffTarget target, Buff buff)
            {
                this.target = target;
                this.buff = buff;
                reference = target.GetBuffReference(buff);
            }
            public BuffCache(BuffReference buffRef)
            {
                reference = buffRef;
            }
            public IBuffTarget GetTarget(LevelEngine level)
            {
                if (target != null)
                    return target;
                if (reference == null)
                    return null;
                EvaluateCache(level);
                return target;
            }
            public Buff GetBuff(LevelEngine level)
            {
                if (buff != null)
                    return buff;
                if (reference == null)
                    return null;
                EvaluateCache(level);
                return buff;
            }
            private void EvaluateCache(LevelEngine level)
            {
                if (reference == null)
                    return;
                target = reference.GetTarget(level);
                buff = reference.GetBuff(level);
            }
            private IBuffTarget target;
            private Buff buff;
            public BuffReference reference;
        }
    }
}
