using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using PVZEngine.Models;
using PVZEngine.Modifiers;

namespace PVZEngine.Buffs
{
    public class BuffList
    {
        #region 增益操作
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
        private bool AddBuffImplement(Buff buff)
        {
            if (buff == null)
                return false;
            buffs.Add(buff);
            AddModifierCaches(buff);
            UpdateModelInsertions();
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
                UpdateModelInsertions();
                buff.OnPropertyChanged -= OnPropertyChangedCallback;
                return true;
            }
            return false;
        }
        #endregion

        #region 属性
        public object CalculateProperty(string name, object value)
        {
            if (buffs.Count == 0)
                return value;

            var modifiers = GetModifierCaches(name);
            return CalculateProperty(modifiers, name, value);
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
        private void OnPropertyChangedCallback(string name)
        {
            OnPropertyChanged?.Invoke(name);
        }
        public string[] GetModifierPropertyNames()
        {
            return modifierCaches.Keys.ToArray();
        }
        #endregion

        #region 模型
        public void UpdateModelInsertions()
        {
            HashSet<NamespaceID> retainModels = new HashSet<NamespaceID>();
            foreach (var buff in buffs)
            {
                foreach (var insertion in buff.GetModelInsertions())
                {
                    retainModels.Add(insertion.key);
                    if (createdModelInsertions.Contains(insertion.key))
                        continue;
                    OnModelInsertionAdded?.Invoke(insertion.anchorName, insertion.key, insertion.modelID);
                    createdModelInsertions.Add(insertion.key);
                }
            }
            for (int i = createdModelInsertions.Count - 1; i >= 0; i--)
            {
                var key = createdModelInsertions[i];
                if (retainModels.Contains(key))
                    continue;
                OnModelInsertionRemoved?.Invoke(key);
                createdModelInsertions.RemoveAt(i);
            }
        }
        #endregion

        #region 修改器缓存
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
        #endregion

        #region 序列化
        public SerializableBuffList ToSerializable()
        {
            return new SerializableBuffList()
            {
                buffs = buffs.ConvertAll(b => b.Serialize())
            };
        }
        public static BuffList FromSerializable(SerializableBuffList serializable, LevelEngine level, IBuffTarget target)
        {
            var buffList = new BuffList();
            foreach (var seriBuff in serializable.buffs)
            {
                var buff = Buff.Deserialize(seriBuff, level, target);
                buff.OnPropertyChanged += buffList.OnPropertyChangedCallback;
                buffList.buffs.Add(buff);

                foreach (var insertion in buff.GetModelInsertions())
                {
                    if (!buffList.createdModelInsertions.Contains(insertion.key))
                    {
                        buffList.createdModelInsertions.Add(insertion.key);
                    }
                }
            }
            buffList.UpdateModifierCaches();
            return buffList;
        }
        #endregion
        public event Action<string, NamespaceID, NamespaceID> OnModelInsertionAdded;
        public event Action<NamespaceID> OnModelInsertionRemoved;
        public event Action<string> OnPropertyChanged;
        private HashSet<string> changedPropertiesBuffer = new HashSet<string>();
        private List<Buff> buffs = new List<Buff>();
        private List<NamespaceID> createdModelInsertions = new List<NamespaceID>();
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
