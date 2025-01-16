using System;
using System.IO;
using System.Text;
using MVZ2.OldSaves;
using NBTUtility;
using UnityEngine;
using UnityEngine.Android;

namespace MVZ2.OldSave
{
    public static class OldSaveDataImporter
    {
        public static OldGlobalSave Import()
        {
            var platform = Application.platform;
            var thisDirectory = Application.persistentDataPath;
            string oldDirectory;
            if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
            {
                var cuerzorDirectory = Path.GetDirectoryName(thisDirectory);
                var localLowDirectory = Path.GetDirectoryName(cuerzorDirectory);
                oldDirectory = Path.Combine(localLowDirectory, "Tocmic Studio", "MinecraftVSZombies2");
            }
            else
            {
                return null;
            }
            var oldSaveDirectory = Path.Combine(oldDirectory, "saves");
            if (!Directory.Exists(oldSaveDirectory))
                return null;
            var usersPath = Path.Combine(oldSaveDirectory, "users.dat");
            OldUserList userList = ImportOldUserList(usersPath);
            if (userList == null)
                return null;

            OldSaveData[] saveDatas = new OldSaveData[userList.usernames.Length];
            for (int i = 0; i < userList.usernames.Length; i++)
            {
                var userName = userList.usernames[i];
                if (string.IsNullOrEmpty(userName))
                    continue;
                var userDirectory = Path.Combine(oldSaveDirectory, $"user{i}");
                var loader = new OldSaveDataLoader(userDirectory);
                var saveData = loader.Load();
                saveDatas[i] = saveData;
            }
            return new OldGlobalSave()
            {
                userList = userList,
                saveDatas = saveDatas
            };
        }
        public static OldUserList ImportOldUserList(string path)
        {
            if (!File.Exists(path))
                return null;
            try
            {
                using var stream = File.Open(path, FileMode.Open);
                return OldUserList.ReadStream(stream);
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured while importing users.dat from the old version: {e}");
                return null;
            }
        }
    }
}
