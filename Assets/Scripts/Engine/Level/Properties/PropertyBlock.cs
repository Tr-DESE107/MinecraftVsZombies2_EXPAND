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
        public void SetProperty(PropertyKey name, object value)
        {
            modifiableProperties.SetProperty(name, value);
        }
        public object GetProperty(PropertyKey name, bool ignoreBuffs = false)
        {
            return modifiableProperties.GetProperty(name, ignoreBuffs);
        }
        public bool TryGetProperty(PropertyKey name, out object result, bool ignoreBuffs = false)
        {
            return modifiableProperties.TryGetProperty(name, out result, ignoreBuffs);
        }
        public T GetProperty<T>(PropertyKey name, bool ignoreBuffs = false)
        {
            return modifiableProperties.GetProperty<T>(name, ignoreBuffs);
        }
        public bool TryGetProperty<T>(PropertyKey name, out T value, bool ignoreBuffs = false)
        {
            return modifiableProperties.TryGetProperty<T>(name, out value, ignoreBuffs);
        }
        public bool RemoveProperty(PropertyKey name)
        {
            return modifiableProperties.RemoveProperty(name);
        }
        public PropertyKey[] GetPropertyNames()
        {
            return modifiableProperties.GetPropertyNames();
        }
        public void UpdateAllModifiedProperties()
        {
            modifiableProperties.UpdateAllModifiedProperties();
        }
        public void UpdateModifiedProperty(PropertyKey name)
        {
            modifiableProperties.UpdateModifiedProperty(name);
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
