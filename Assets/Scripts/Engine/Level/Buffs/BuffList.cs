using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace PVZEngine.Buffs
{
    public class BuffList
    {
        public bool AddBuff(Buff buff)
        {
            changedPropertiesBuffer.Clear();
            if (AddBuffImplement(buff))
            {
                foreach (var prop in changedPropertiesBuffer)
                {
                    OnPropertyChangedCallback(prop);
                }
                return true;
            }
            return false;
        }
        public bool RemoveBuff(Buff buff)
        {
            changedPropertiesBuffer.Clear();
            if (RemoveBuffImplement(buff))
            {
                foreach (var prop in changedPropertiesBuffer)
                {
                    OnPropertyChangedCallback(prop);
                }
                return true;
            }
            return false;
        }
        public int RemoveBuffs(IEnumerable<Buff> buffs)
        {
            if (buffs == null)
                return 0;

            changedPropertiesBuffer.Clear();
            int count = 0;
            foreach (var buff in buffs)
            {
                count += RemoveBuffImplement(buff) ? 1 : 0;
            }
            foreach (var prop in changedPropertiesBuffer)
            {
                OnPropertyChangedCallback(prop);
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
        public string[] GetModifierPropertyNames()
        {
            return modifierCaches.Keys.ToArray();
        }
        public object CalculateProperty(string name, object value)
        {
            if (buffs.Count == 0)
                return value;

            var modifiers = GetModifierCaches(name);
            return CalculateProperty(modifiers, name, value);
        }
        public SerializableBuffList ToSerializable()
        {
            return new SerializableBuffList()
            {
                buffs = buffs.ConvertAll(b => b.Serialize())
            };
        }
        private object CalculateProperty(List<BuffModifierItem> modifiers, string name, object value)
        {
            if (modifiers == null || modifiers.Count == 0)
                return value;

            var calculators = modifiers.Select(p => p.modifier.GetCalculator()).Where(p => p != null).Distinct();
            ModifierCalculator calculator = null;
            foreach (var calc in calculators)
            {
                if (calculator != null)
                    throw new MultipleValueModifierException($"Modifiers of property {name} has multiple different calculators: {string.Join(',', calculators)}");
                calculator = calc;
            }
            if (calculator == null)
                throw new NullReferenceException($"Calculator for property {name} does not exists.");
            return calculator.Calculate(value, modifiers);
        }
        public bool AddBuffImplement(Buff buff)
        {
            if (buff == null)
                return false;
            buffs.Add(buff);
            AddModifierCaches(buff);
            buff.OnPropertyChanged += OnPropertyChangedCallback;
            return true;
        }
        private bool RemoveBuffImplement(Buff buff)
        {
            if (buff == null)
                return false;
            if (buffs.Remove(buff))
            {
                buff.RemoveFromTarget();
                RemoveModifierCaches(buff);
                buff.OnPropertyChanged -= OnPropertyChangedCallback;
                return true;
            }
            return false;
        }
        private void OnPropertyChangedCallback(string name)
        {
            OnPropertyChanged?.Invoke(name);
        }
        private void AddModifierCaches(Buff buff)
        {
            foreach (var modifier in buff.GetModifiers())
            {
                var name = modifier.PropertyName;
                if (!modifierCaches.TryGetValue(name, out var list))
                {
                    list = new List<BuffModifierItem>();
                    modifierCaches.Add(name, list);
                }
                list.Add(new BuffModifierItem(buff, modifier));
                changedPropertiesBuffer.Add(name);
            }
        }
        private void RemoveModifierCaches(Buff buff)
        {
            foreach (var modifier in buff.GetModifiers())
            {
                var name = modifier.PropertyName;
                if (modifierCaches.TryGetValue(name, out var list))
                {
                    list.RemoveAll(b => b.buff == buff);
                }
                changedPropertiesBuffer.Add(name);
            }
        }
        private void UpdateModifierCaches()
        {
            foreach (var buff in buffs)
            {
                AddModifierCaches(buff);
            }
        }
        private List<BuffModifierItem> GetModifierCaches(string name)
        {
            if (modifierCaches.TryGetValue(name, out var list))
                return list;
            return null;
        }
        public static BuffList FromSerializable(SerializableBuffList serializable, LevelEngine level, IBuffTarget target)
        {
            var buffList = new BuffList()
            {
                buffs = serializable.buffs.ConvertAll(b => Buff.Deserialize(b, level, target))
            };
            buffList.UpdateModifierCaches();
            return buffList;
        }
        public event Action<string> OnPropertyChanged;
        private HashSet<string> changedPropertiesBuffer = new HashSet<string>();
        private List<Buff> buffs = new List<Buff>();
        private Dictionary<string, List<BuffModifierItem>> modifierCaches = new Dictionary<string, List<BuffModifierItem>>();
    }
    public class MultipleValueModifierException : Exception
    {
        public MultipleValueModifierException(string message) : base(message)
        {
        }
    }
    public struct PropertyCalculateResult
    {
        public string name;
        public object value;
    }
}
