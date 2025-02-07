using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Level;
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
            for (int i = buffs.Count - 1; i >= 0; i --)
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
        public object CalculateProperty(PropertyKey name, object value)
        {
            if (buffs.Count == 0)
                return value;

            modifierItemBuffer.Clear();
            GetModifierItems(name, modifierItemBuffer);
            return modifierItemBuffer.CalculateProperty(value);
        }
        private void OnPropertyChangedCallback(PropertyKey name)
        {
            OnPropertyChanged?.Invoke(name);
        }
        public PropertyKey[] GetModifierPropertyNames()
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
        public void GetModifierItems(PropertyKey name, List<ModifierContainerItem> results)
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
        public event Action<PropertyKey> OnPropertyChanged;
        private HashSet<PropertyKey> changedPropertiesBuffer = new HashSet<PropertyKey>();
        private List<Buff> updateBuffer = new List<Buff>();
        private List<Buff> buffs = new List<Buff>();
        private List<NamespaceID> createdModelInsertions = new List<NamespaceID>();
        private Dictionary<PropertyKey, List<ModifierContainerItem>> modifierCaches = new Dictionary<PropertyKey, List<ModifierContainerItem>>();
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
