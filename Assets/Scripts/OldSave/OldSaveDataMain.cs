using System.Collections.Generic;
using System.IO;
using NBTUtility;

namespace MVZ2.OldSaves
{
    public class OldSaveData
    {
        public OldSaveDataMain main;
        public OldSaveDataAchievements achievements;
        public OldSaveDataEndless endless;
    }
    public class OldSaveDataMain
    {
        public byte version;
        public byte currentLevel;
        public byte[] levelDifficulties;
        public string lastMap;
        public byte sidePlot;
        public bool moneyFound;
        public int money;
        public bool starshardLearnt;
        public bool triggerLearnt;
        public byte cardSlots;
        public byte starshardSlots;
        public bool obsidianFirstAid;
        public short upgrades;
        public bool nightmare;

        public static OldSaveDataMain FromNBT(NBTData nbt)
        {
            byte version = nbt.LoadByte("version", 0);
            if (version == 0)
            {
                return LoadVersion0(nbt);
            }
            return LoadVersion1(nbt);
        }
        private static OldSaveDataMain LoadVersion0(NBTData nbt)
        {
            byte currentLevel = nbt.LoadByte("mvz2:current_level", 0);
            byte[] levelDifficulties = nbt.LoadByteArray("mvz2:level_difficulties", new byte[132]);
            string lastMap = nbt.LoadString("mvz2:last_map", "mvz2:halloween_map");
            byte sidePlot = nbt.LoadByte("mvz2:side_plot", 0);
            bool moneyFound = nbt.LoadBool("mvz2:money_found", false);
            int money = nbt.LoadInt("mvz2:money", 0);
            bool starshardLearnt = nbt.LoadBool("mvz2:starshard_learnt", false);
            bool triggerLearnt = nbt.LoadBool("mvz2:trigger_learnt", false);
            byte cardSlots = nbt.LoadByte("mvz2:card_slots", 0);
            byte starshardSlots = nbt.LoadByte("mvz2:starshard_slots", 0);
            bool obsidianFirstAid = nbt.LoadBool("mvz2:obsidian_first_aid", false);
            short upgrades = nbt.LoadShort("mvz2:taboos", 0);
            return new OldSaveDataMain()
            {
                version = 0,
                currentLevel = currentLevel,
                levelDifficulties = levelDifficulties,
                lastMap = lastMap,
                sidePlot = sidePlot,
                moneyFound = moneyFound,
                money = money,
                starshardLearnt = starshardLearnt,
                triggerLearnt = triggerLearnt,
                cardSlots = cardSlots,
                starshardSlots = starshardSlots,
                obsidianFirstAid = obsidianFirstAid,
                upgrades = upgrades,
                nightmare = false
            };
        }
        private static OldSaveDataMain LoadVersion1(NBTData nbt)
        {
            byte currentLevel = nbt.LoadByte("current_level", 0);
            byte[] levelDifficulties = nbt.LoadByteArray("level_difficulties", new byte[132]);
            string lastMap = nbt.LoadString("last_map", "mvz2:halloween_map");
            byte sidePlot = nbt.LoadByte("side_plot", 0);
            bool moneyFound = nbt.LoadBool("money_found", false);
            int money = nbt.LoadInt("money", 0);
            bool starshardLearnt = nbt.LoadBool("starshard_learnt", false);
            bool triggerLearnt = nbt.LoadBool("trigger_learnt", false);
            byte cardSlots = nbt.LoadByte("card_slots", 0);
            byte starshardSlots = nbt.LoadByte("starshard_slots", 0);
            bool obsidianFirstAid = nbt.LoadBool("obsidian_first_aid", false);
            short upgrades = nbt.LoadShort("upgrades", 0);
            bool nightmare = nbt.LoadBool("nightmare", false);
            return new OldSaveDataMain()
            {
                version = 1,
                currentLevel = currentLevel,
                levelDifficulties = levelDifficulties,
                lastMap = lastMap,
                sidePlot = sidePlot,
                moneyFound = moneyFound,
                money = money,
                starshardLearnt = starshardLearnt,
                triggerLearnt = triggerLearnt,
                cardSlots = cardSlots,
                starshardSlots = starshardSlots,
                obsidianFirstAid = obsidianFirstAid,
                upgrades = upgrades,
                nightmare = nightmare
            };
        }
    }
    public class OldSaveDataAchievements
    {
        public byte version;
        public byte[] earned;
        public static OldSaveDataAchievements FromNBT(NBTData nbt)
        {
            byte version = nbt.LoadByte("version", 0);
            byte[] earned = nbt.LoadByteArray("earned", new byte[0]);
            return new OldSaveDataAchievements()
            {
                version = 0,
                earned = earned
            };
        }
    }
    public class OldSaveDataEndless
    {
        public byte version;
        public Dictionary<string, Record> flags;
        public static OldSaveDataEndless FromNBT(NBTData nbt)
        {
            byte version = nbt.LoadByte("version", 0);
            Dictionary<string, Record> flags = new Dictionary<string, Record>();
            if (nbt.TryGetValue("flags", out var flagsData))
            {
                foreach (var key in flagsData.Keys)
                {
                    if (flagsData.TryGetValue(key, out var endlessData))
                    {
                        var current = endlessData.LoadInt("current", 0);
                        var max = endlessData.LoadInt("max", 0);
                        flags[key] = new Record()
                        {
                            current = current,
                            max = max
                        };
                    }
                    else
                    {
                        flags[key] = new Record();
                    }
                }
            }
            return new OldSaveDataEndless()
            {
                version = 0,
                flags = flags
            };
        }
        public class Record
        {
            public int current;
            public int max;
        }
    }
}
