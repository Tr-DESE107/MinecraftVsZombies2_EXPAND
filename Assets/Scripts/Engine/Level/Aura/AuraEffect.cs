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
            var missingEntities = buffDict.Where(p => !p.Key.Exists() || !targetsBuffer.Contains(p.Key)).ToArray();
            foreach (var pair in missingEntities)
            {
                RemoveTargetBuff(pair.Key, pair.Value);
            }
        }
        public SerializableAuraEffect ToSerializable()
        {
            return new SerializableAuraEffect()
            {
                id = ID,
                buffs = buffDict.Select(p => p.Key.GetBuffReference(p.Value)).ToArray(),
                updateTimer = updateTimer,
            };
        }
        public void LoadFromSerializable(LevelEngine level, SerializableAuraEffect serializable)
        {
            updateTimer = serializable.updateTimer;
            buffDict = serializable.buffs
                .Select(b => (b.GetTarget(level), b.GetBuff(level)))
                .Where(b => b.Item1 != null && b.Item2 != null)
                .ToDictionary(p => p.Item1, p => p.Item2);
        }
        private Buff AddTargetBuff(IBuffTarget entity)
        {
            var buff = entity.CreateBuff(Definition.BuffID);
            entity.AddBuff(buff);
            buffDict.Add(entity, buff);
            return buff;
        }
        private Buff GetTargetBuff(IBuffTarget entity)
        {
            return buffDict.TryGetValue(entity, out var buff) ? buff : null;
        }
        private bool RemoveTargetBuff(IBuffTarget entity, Buff buff)
        {
            if (buffDict.Remove(entity))
            {
                entity.RemoveBuff(buff);
                return true;
            }
            return false;
        }
        private void ClearBuffs()
        {
            foreach (var pair in buffDict)
            {
                pair.Key.RemoveBuff(pair.Value);
            }
            buffDict.Clear();
        }
        public int ID { get; }
        public AuraEffectDefinition Definition { get; }
        public IAuraSource Source { get; }
        private FrameTimer updateTimer;
        private Dictionary<IBuffTarget, Buff> buffDict = new Dictionary<IBuffTarget, Buff>();
        private List<IBuffTarget> targetsBuffer = new List<IBuffTarget>();
    }
}
