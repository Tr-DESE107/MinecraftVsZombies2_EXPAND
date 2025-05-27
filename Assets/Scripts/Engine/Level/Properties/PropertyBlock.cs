using System;

namespace PVZEngine.Level
{
    public class PropertyBlock
    {
        public PropertyBlock(IPropertyModifyTarget container)
        {
            modifiableProperties = new ModifiableProperties(container);
        }
        private PropertyBlock()
        {
        }

        #region 可修改属性
        public void SetProperty<T>(PropertyKey<T> name, T value)
        {
            modifiableProperties.SetProperty(name, value);
        }
        public void SetPropertyObject(IPropertyKey name, object value)
        {
            modifiableProperties.SetPropertyObject(name, value);
        }
        public T GetProperty<T>(PropertyKey<T> name, bool ignoreBuffs = false)
        {
            return modifiableProperties.GetProperty<T>(name, ignoreBuffs);
        }
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value, bool ignoreBuffs = false)
        {
            return modifiableProperties.TryGetProperty<T>(name, out value, ignoreBuffs);
        }
        public bool RemoveProperty<T>(PropertyKey<T> name)
        {
            return modifiableProperties.RemoveProperty<T>(name);
        }
        public IPropertyKey[] GetPropertyNames()
        {
            return modifiableProperties.GetPropertyNames();
        }
        public void UpdateAllModifiedProperties()
        {
            modifiableProperties.UpdateAllModifiedProperties();
        }
        public void UpdateModifiedProperty<T>(PropertyKey<T> name)
        {
            modifiableProperties.UpdateModifiedProperty<T>(name);
        }
        public void UpdateModifiedProperty(IPropertyKey name)
        {
            modifiableProperties.UpdateModifiedPropertyObject(name);
        }
        #endregion

        public SerializablePropertyBlock ToSerializable()
        {
            return new SerializablePropertyBlock()
            {
                modifiable = modifiableProperties.ToSerializable(),
            };
        }
        public static PropertyBlock FromSerializable(SerializablePropertyBlock seri, IPropertyModifyTarget container)
        {
            var block = new PropertyBlock();
            block.modifiableProperties = ModifiableProperties.FromSerializable(seri.modifiable, container);
            return block;
        }

        private ModifiableProperties modifiableProperties;
    }
    [Serializable]
    public class SerializablePropertyBlock
    {
        public SerializableModifiableProperties modifiable;
    }
}
