namespace PVZEngine.Buffs
{
    [PropertyRegistryRegion(PropertyRegions.buff)]
    public static class EngineBuffProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }

        #region 清晰度
        public static readonly PropertyMeta<int> BUFF_LEVEL = Get<int>("clarity");
        public static int GetClarity(this Buff buff)
        {
            return buff.GetProperty<int>(BUFF_LEVEL);
        }
        #endregion

        #region 极性
        public static readonly PropertyMeta<int> POLARITY = Get<int>("polarity");
        public static int IsPolarity(this Buff entity)
        {
            return entity.GetProperty<int>(POLARITY);
        }
        #endregion
    }
}
