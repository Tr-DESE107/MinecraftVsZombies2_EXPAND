using MVZ2.Vanilla.Saves;
using MVZ2Logic.Games;
using MVZ2Logic.Modding;
using MVZ2Logic.Saves;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
        }
        public override void Init(IGlobalGame game)
        {
            base.Init(game);
            RegisterSerializableType<SerializableVanillaSaveData>();
        }

        #region 存档
        public override ModSaveData CreateSaveData()
        {
            return new VanillaSaveData(spaceName);
        }
        public override ModSaveData LoadSaveData(string json)
        {
            var serializable = Deserialize<SerializableVanillaSaveData>(json);
            return serializable.Deserialize();
        }
        #endregion

        public const string spaceName = "mvz2";
    }
}
