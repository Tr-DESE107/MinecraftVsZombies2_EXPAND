using System;
using PVZEngine.Modifiers;

namespace PVZEngine.Level
{
    public class PropertyBlock
    {
        public PropertyBlock(IPropertyModifyTarget container)
        {
            modifiableProperties = new ModifiableProperties(container);
            fieldProperties = new PropertyCategoryDictionary();
        }
        private PropertyBlock()
        {
        }

        #region 可修改属性
        public void SetProperty(string name, object value)
        {
            modifiableProperties.SetProperty(name, value);
        }
        public object GetProperty(string name, bool ignoreBuffs = false)
        {
            return modifiableProperties.GetProperty(name, ignoreBuffs);
        }
        public bool TryGetProperty(string name, out object result, bool ignoreBuffs = false)
        {
            return modifiableProperties.TryGetProperty(name, out result, ignoreBuffs);
        }
        public T GetProperty<T>(string name, bool ignoreBuffs = false)
        {
            return modifiableProperties.GetProperty<T>(name, ignoreBuffs);
        }
        public bool TryGetProperty<T>(string name, out T value, bool ignoreBuffs = false)
        {
            return modifiableProperties.TryGetProperty<T>(name, out value, ignoreBuffs);
        }
        public bool RemoveProperty(string name)
        {
            return modifiableProperties.RemoveProperty(name);
        }
        public string[] GetPropertyNames()
        {
            return modifiableProperties.GetPropertyNames();
        }
        public void UpdateAllModifiedProperties()
        {
            modifiableProperties.UpdateAllModifiedProperties();
        }
        public void UpdateModifiedProperty(string name)
        {
            modifiableProperties.UpdateModifiedProperty(name);
        }
        #endregion

        #region 字段
        public void SetField(string category, string name, object value)
        {
            fieldProperties.SetProperty(category, name, value);
        }
        public object GetField(string category, string name)
        {
            return fieldProperties.GetProperty(category, name);
        }
        public bool TryGetField(string category, string name, out object result)
        {
            return fieldProperties.TryGetProperty(category, name, out result);
        }
        public T GetField<T>(string category, string name)
        {
            return fieldProperties.GetProperty<T>(category, name);
        }
        public bool TryGetField<T>(string category, string name, out T value)
        {
            return fieldProperties.TryGetProperty<T>(category, name, out value);
        }
        public bool RemoveField(string category, string name)
        {
            return fieldProperties.RemoveProperty(category, name);
        }
        public string[] GetFieldNames(string category)
        {
            return fieldProperties.GetPropertyNames(category);
        }
        public string[] GetFieldCategories()
        {
            return fieldProperties.GetCategories();
        }
        #endregion

        public SerializablePropertyBlock ToSerializable()
        {
            return new SerializablePropertyBlock()
            {
                modifiable = modifiableProperties.ToSerializable(),
                fields = fieldProperties.ToSerializable()
            };
        }
        public static PropertyBlock FromSerializable(SerializablePropertyBlock seri, IPropertyModifyTarget container)
        {
            var block = new PropertyBlock();
            block.modifiableProperties = ModifiableProperties.FromSerializable(seri.modifiable, container);
            block.fieldProperties = PropertyCategoryDictionary.FromSerializable(seri.fields);
            return block;
        }

        private ModifiableProperties modifiableProperties;
        private PropertyCategoryDictionary fieldProperties;
    }
    [Serializable]
    public class SerializablePropertyBlock
    {
        public SerializableModifiableProperties modifiable;
        public SerializablePropertyCategoryDictionary fields;
    }
}
