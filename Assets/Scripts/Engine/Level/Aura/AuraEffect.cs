using System.Collections.Generic;
using System.Linq;
using PVZEngine.Buffs;
using PVZEngine.Level;
using Tools;

namespace PVZEngine.Auras
{
    public class AuraEffect
    {
        public AuraEffect(AuraEffectDefinition definition, int id)
        {
            ID = id;
            Definition = definition;
            updateTimer = new FrameTimer(Definition.UpdateInterval);
        }
        public void UpdateAuraInterval(LevelEngine level)
        {
            updateTimer.Run();
            if (updateTimer.Expired)
            {
                updateTimer.Reset();
                UpdateAura(level);
            }
        }
        public void PostAdd(LevelEngine level)
        {
            Definition.PostAdd(level, this);
        }
        public void PostRemove(LevelEngine level)
        {
            Definition.PostRemove(level, this);
        }
        public void UpdateAura(LevelEngine level)
        {
            var targets = Definition.GetAuraTargets(level, this).Where(t => t != null);
            foreach (var target in targets)
            {
                UpdateBuffTarget(target);
            }
            var missingEntities = buffDict.Where(p => !p.Key.Exists()).ToArray();
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
        private void UpdateBuffTarget(IBuffTarget target)
        {
            var buff = GetTargetBuff(target);
            if (Definition.CheckCondition(this, target))
            {
                if (buff == null)
                {
                    buff = AddTargetBuff(target);
                }
                Definition.UpdateTargetBuff(this, target, buff);
            }
            else
            {
                if (buff != null)
                {
                    RemoveTargetBuff(target, buff);
                }
            }
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
        public int ID { get; }
        public AuraEffectDefinition Definition { get; }
        private FrameTimer updateTimer;
        private Dictionary<IBuffTarget, Buff> buffDict = new Dictionary<IBuffTarget, Buff>();
    }
}
