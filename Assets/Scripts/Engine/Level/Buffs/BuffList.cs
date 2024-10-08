using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;
using PVZEngine.Serialization;

namespace PVZEngine.Level
{
    public class BuffList
    {
        public bool AddBuff(Buff buff)
        {
            if (buff == null)
                return false;
            buffs.Add(buff);
            buff.Level.AddTriggers(buff.GetTriggers());
            return true;
        }
        public bool RemoveBuff(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                buff.Level.RemoveTriggers(buff.GetTriggers());
                return true;
            }
            return false;
        }
        public int RemoveBuffs(IEnumerable<Buff> buffs)
        {
            if (buffs == null)
                return 0;
            int count = 0;
            foreach (var buff in buffs)
            {
                count += RemoveBuff(buff) ? 1 : 0;
            }
            return count;
        }
        public bool HasBuff<T>() where T : BuffDefinition
        {
            return buffs.Any(b => b.Definition is T);
        }
        public bool HasBuff(BuffDefinition buffDef)
        {
            return buffs.Any(b => b.Definition == buffDef);
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        public Buff[] GetBuffs<T>() where T : BuffDefinition
        {
            return buffs.Where(b => b.Definition is T).ToArray();
        }
        public Buff[] GetBuffs(BuffDefinition buffDef)
        {
            return buffs.Where(b => b.Definition == buffDef).ToArray();
        }
        public Buff[] GetAllBuffs()
        {
            return buffs.ToArray();
        }
        public object CalculateProperty(string name, object value)
        {
            if (buffs.Count == 0)
                return value;

            var modifiers = buffs.SelectMany(buff => buff.GetModifiers(name).Select(modifier => new BuffModifierItem(buff, modifier)));
            if (modifiers.Count() == 0)
                return value;

            var modifierTypes = modifiers.Select(p => p.modifier.GetType()).Distinct();

            if (modifierTypes.Count() > 1)
                throw new MultipleValueModifierException($"Modifiers of property {name} has multiple modifiers with different types : {string.Join(',', modifierTypes)}");

            var modifierType = modifierTypes.First();
            var calculator = ModifierMap.GetCalculator(modifierType);

            if (calculator == null)
                throw new NullReferenceException($"Calculator for modifier type {modifierType} does not exists.");

            return calculator.Calculate(value, modifiers);
        }
        public SerializableBuffList ToSerializable()
        {
            return new SerializableBuffList()
            {
                buffs = buffs.ConvertAll(b => b.Serialize())
            };
        }
        public static BuffList FromSerializable(SerializableBuffList buffList, LevelEngine level, IBuffTarget target)
        {
            return new BuffList()
            {
                buffs = buffList.buffs.ConvertAll(b => Buff.Deserialize(b, level, target))
            };
        }
        private List<Buff> buffs = new List<Buff>();
    }
    public class MultipleValueModifierException : Exception
    {
        public MultipleValueModifierException(string message) : base(message)
        {
        }
    }
}
