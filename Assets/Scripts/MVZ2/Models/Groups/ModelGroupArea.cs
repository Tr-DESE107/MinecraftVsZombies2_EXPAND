﻿namespace MVZ2.Models
{
    public class ModelGroupArea : ModelGroupRenderer
    {
        #region 序列化
        public override SerializableModelGroup ToSerializable()
        {
            var serializable = new SerializableModelGroupArea();
            SaveToSerializableRenderer(serializable);
            return serializable;
        }
        #endregion
    }
    public class SerializableModelGroupArea : SerializableModelGroupRenderer
    {
    }
}