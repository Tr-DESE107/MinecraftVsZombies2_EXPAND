using MVZ2.GameContent.Implements;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
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
        public override void Init(IGame game)
        {
            base.Init(game);


            // 回调。
            ImplementCallbacks(new GemStageImplements());
            ImplementCallbacks(new BlueprintImplements());
            ImplementCallbacks(new StatsImplements());
            ImplementCallbacks(new EntityImplements());
            ImplementCallbacks(new IZombieImplements());
            ImplementCallbacks(new DifficultyImplements());
            ImplementCallbacks(new CartToMoneyImplements());
            ImplementCallbacks(new TalkActionImplements());
            ImplementCallbacks(new BlueprintRecommendImplements());
            ImplementCallbacks(new WaterImplements());
            ImplementCallbacks(new CloudImplements());
            ImplementCallbacks(new CarrierImplements());
            ImplementCallbacks(new AchievementsImplements());
            ImplementCallbacks(new DebugImplements());
            ImplementCallbacks(new RandomChinaImplements());
            ImplementCallbacks(new AlmanacImplements());
        }
        public override void PostGameInit()
        {
            base.PostGameInit();
            SerializeHelper.RegisterClass<SerializableVanillaSaveData>();
        }

        #region 存档
        public override ModSaveData CreateSaveData()
        {
            return new VanillaSaveData(spaceName);
        }
        public override ModSaveData LoadSaveData(string json)
        {
            var serializable = SerializeHelper.FromBson<SerializableVanillaSaveData>(json);
            return serializable.Deserialize();
        }
        #endregion

        public const string spaceName = "mvz2";
    }
}
