namespace PVZEngine.Buffs
{
    public class ModelInsertion
    {
        public ModelInsertion(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            this.anchorName = anchorName;
            this.key = key;
            this.modelID = modelID;
        }
        public string anchorName;
        public NamespaceID key;
        public NamespaceID modelID;
    }
}
