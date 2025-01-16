using System;
using System.IO;
using System.Text;
using MVZ2.OldSaves;
using NBTUtility;
using UnityEngine;

namespace MVZ2.OldSave
{
    public class OldSaveDataLoader
    {
        public OldSaveDataLoader(string directory)
        {
            this.directory = directory;
        }
        public OldSaveData Load()
        {
            var main = LoadMain();
            var achievements = LoadAchievements();
            var endless = LoadEndless();
            return new OldSaveData()
            {
                main = main,
                achievements = achievements,
                endless = endless
            };
        }
        private OldSaveDataMain LoadMain()
        {
            var dir = GetGlobalSaveDataDirectory();
            var path = Path.Combine(dir, "main.dat");
            if (!File.Exists(path))
                return null;

            try
            {
                using var stream = File.Open(path, FileMode.Open);
                NBTReader reader = new NBTReader(stream, Encoding.UTF8);
                NBTData nbt = NBTMapper.ToObject(reader);
                return OldSaveDataMain.FromNBT(nbt);
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occured while importing main.dat from the old version: {ex}");
                return null;
            }
        }
        private OldSaveDataAchievements LoadAchievements()
        {
            var dir = GetGlobalSaveDataDirectory();
            var path = Path.Combine(dir, "achievements.dat");
            if (!File.Exists(path))
                return null;

            try
            {
                using var stream = File.Open(path, FileMode.Open);
                NBTReader reader = new NBTReader(stream, Encoding.UTF8);
                NBTData nbt = NBTMapper.ToObject(reader);
                return OldSaveDataAchievements.FromNBT(nbt);
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occured while importing achievements.dat from the old version: {ex}");
                return null;
            }
        }
        private OldSaveDataEndless LoadEndless()
        {
            var dir = GetGlobalSaveDataDirectory();
            var path = Path.Combine(dir, "endless.dat");
            if (!File.Exists(path))
                return null;

            try
            {
                using var stream = File.Open(path, FileMode.Open);
                NBTReader reader = new NBTReader(stream, Encoding.UTF8);
                NBTData nbt = NBTMapper.ToObject(reader);
                return OldSaveDataEndless.FromNBT(nbt);
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occured while importing endless.dat from the old version: {ex}");
                return null;
            }
        }
        private string GetGlobalSaveDataDirectory()
        {
            return Path.Combine(directory, "global", "mvz2");
        }

        private string directory;

    }
}
