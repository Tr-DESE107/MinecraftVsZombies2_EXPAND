using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace PVZEngine.Buffs
{
    public class BuffList : IEnumerable<Buff>
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

        #region 移除
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
        #endregion

        #region 移除多个
        public int RemoveBuffs(BuffDefinition buffDef)
        {
            if (buffDef == null)
                return 0;

            changedPropertiesBuffer.Clear();
            int count = 0;
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                var buff = buffs[i];
                if (buff.Definition != buffDef)
                    continue;
                count += RemoveBuffImplement(buff) ? 1 : 0;
            }
            foreach (var prop in changedPropertiesBuffer)
            {
                OnPropertyChangedCallback(prop);
            }
            return count;
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
        public int RemoveBuffs<T>() where T : BuffDefinition
        {
            changedPropertiesBuffer.Clear();
            int count = 0;
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                var buff = buffs[i];
                if (buff.Definition is not T)
                    continue;
                count += RemoveBuffImplement(buff) ? 1 : 0;
            }
            foreach (var prop in changedPropertiesBuffer)
            {
                OnPropertyChangedCallback(prop);
            }
            return count;
        }
        #endregion

        #region 包含
        public bool HasBuff<T>() where T : BuffDefinition
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition is T)
                    return true;
            }
            return false;
        }
        public bool HasBuff(NamespaceID id)
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition.GetID() == id)
                    return true;
            }
            return false;
        }
        public bool HasBuff(BuffDefinition buffDef)
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition == buffDef)
                    return true;
            }
            return false;
        }
        public bool HasBuff(Buff buff)
        {
            return buffs.Contains(buff);
        }
        #endregion

        #region 获取单个
        public Buff GetBuff(long id)
        {
            foreach (var buff in buffs)
            {
                if (buff.ID == id)
                    return buff;
            }
            return null;
        }
        public Buff GetFirstBuff<T>() where T : BuffDefinition
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition is T)
                {
                    return buff;
                }
            }
            return null;
        }
        #endregion

        #region 获取多个
        public Buff[] GetBuffs<T>() where T : BuffDefinition
        {
            var list = new List<Buff>();
            GetBuffsNonAlloc<T>(list);
            return list.ToArray();
        }
        public Buff[] GetBuffs(BuffDefinition definition)
        {
            var list = new List<Buff>();
            GetBuffsNonAlloc(definition, list);
            return list.ToArray();
        }
        public void GetBuffsNonAlloc<T>(List<Buff> results) where T : BuffDefinition
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition is T)
                {
                    results.Add(buff);
                }
            }
        }
        public void GetBuffsNonAlloc(BuffDefinition definition, List<Buff> results)
        {
            foreach (var buff in buffs)
            {
                if (buff.Definition == definition)
                {
                    results.Add(buff);
                }
            }
        }
        #endregion

        #region 获取全部
        public void GetAllBuffs(List<Buff> results)
        {
            results.AddRange(buffs);
        }
        #endregion

        public void Update()
        {
            updateBuffer.Clear();
            GetAllBuffs(updateBuffer);
            foreach (var buff in updateBuffer)
            {
                buff.Update();
            }
        }
        private bool AddBuffImplement(Buff buff)
        {
            if (buff == null)
                return false;
            buffs.Add(buff);
            AddModifierCaches(buff);
            OnBuffAdded?.Invoke(buff);
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
                OnBuffRemoved?.Invoke(buff);
                buff.OnPropertyChanged -= OnPropertyChangedCallback;
                return true;
            }
            return false;
        }
        #endregion

        #region 属性
        public object CalculateProperty<T>(PropertyKey<T> name, T value)
        {
            if (buffs.Count == 0)
                return value;

            modifierItemBuffer.Clear();
            GetModifierItems(name, modifierItemBuffer);
            return modifierItemBuffer.CalculateProperty(value);
        }
        private void OnPropertyChangedCallback(IPropertyKey name)
        {
            OnPropertyChanged?.Invoke(name);
        }
        public IPropertyKey[] GetModifierPropertyNames()
        {
            return modifierCaches.Keys.ToArray();
        }
        #endregion

        #region 修改器缓存
        public void GetModifierItems(IPropertyKey name, List<ModifierContainerItem> results)
        {
            if (modifierCaches.TryGetValue(name, out var list))
            {
                results.AddRange(list);
            }
        }
        private void AddModifierCaches(Buff buff)
        {
            foreach (var modifier in buff.GetModifiers())
            {
                var name = modifier.PropertyName;
                if (!modifierCaches.TryGetValue(name, out var list))
                {
                    list = new List<ModifierContainerItem>();
                    modifierCaches.Add(name, list);
                }
                list.Add(new ModifierContainerItem(buff, modifier));
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
                    list.RemoveAll(b => b.container == buff);
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
            }
            buffList.UpdateModifierCaches();
            return buffList;
        }
        public void LoadAuras(SerializableBuffList serializable, LevelEngine level)
        {
            foreach (var buff in buffs)
            {
                if (buff == null)
                    continue;
                var seriBuff = serializable.buffs.FirstOrDefault(b => b.id == buff.ID);
                if (seriBuff == null)
                    continue;
                buff.LoadAuras(seriBuff, level);
            }
        }
        #endregion

        IEnumerator<Buff> IEnumerable<Buff>.GetEnumerator()
        {
            return buffs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return buffs.GetEnumerator();
        }

        public event Action<Buff> OnBuffAdded;
        public event Action<Buff> OnBuffRemoved;
        public event Action<IPropertyKey> OnPropertyChanged;

        private List<Buff> updateBuffer = new List<Buff>();
        private List<Buff> buffs = new List<Buff>();
        private HashSet<IPropertyKey> changedPropertiesBuffer = new HashSet<IPropertyKey>();
        private Dictionary<IPropertyKey, List<ModifierContainerItem>> modifierCaches = new Dictionary<IPropertyKey, List<ModifierContainerItem>>(new PropertyKeyComparer());
        private List<ModifierContainerItem> modifierItemBuffer = new List<ModifierContainerItem>();
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
